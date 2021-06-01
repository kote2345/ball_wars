using Sandbox.Internal.JsonConvert;
using Sandbox.Rcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox
{
	[Library]
	public class BallController : BasePlayerController
	{
		public float AirAcceleration { get; set; } = 6;
		public float StopSpeed { get; set; } = 100.0f;
		public float GroundNormalZ { get; set; } = 0.707f;
		public float StepSize { get; set; } = 18.0f;
		public float MaxNonJumpVelocity { get; set; } = 140.0f;
		public float BodyGirth { get; set; } = 32.0f;
		public float BodyHeight { get; set; } = 72.0f;
		public float EyeHeight { get; set; } = 64.0f;
		public float Gravity { get; set; } = 800.0f;
		public float AirControl { get; set; } = 30.0f;
		public bool AutoJump { get; set; } = false;
		public float Speed { get; set; } = 1f;
		public bool isEnergyNull = false;

		public Unstuck Unstuck;


		public BallController()
		{
			Unstuck = new Unstuck( this );
		}

		/// <summary>
		/// This is temporary, get the hull size for the player's collision
		/// </summary>
		public override BBox GetHull()
		{
			var girth = BodyGirth * 0.5f;
			var mins = new Vector3( -girth, -girth, 0 );
			var maxs = new Vector3( +girth, +girth, BodyHeight );

			return new BBox( mins, maxs );
		}

		// Duck body height 32
		// Eye Height 64
		// Duck Eye Height 28

		protected Vector3 mins;
		protected Vector3 maxs;

		public virtual void SetBBox( Vector3 mins, Vector3 maxs )
		{
			if ( this.mins == mins && this.maxs == maxs )
				return;

			this.mins = mins;
			this.maxs = maxs;
		}

		/// <summary>
		/// Update the size of the bbox. We should really trigger some shit if this changes.
		/// </summary>
		public virtual void UpdateBBox()
		{
			var girth = BodyGirth * 0.5f;

			var mins = new Vector3( -girth, -girth, 0 ) * Pawn.Scale;
			var maxs = new Vector3( +girth, +girth, BodyHeight ) * Pawn.Scale;


			SetBBox( mins, maxs );
		}

		protected float SurfaceFriction;

		public override void Simulate()
		{
			EyePosLocal = Vector3.Up * (EyeHeight * Pawn.Scale);
			UpdateBBox();

			EyePosLocal += TraceOffset;

			RestoreGroundPos();

			if ( Unstuck.TestAndFix() )
				return;


			if ( AutoJump ? Input.Down( InputButton.Jump ) : Input.Pressed( InputButton.Jump ) )
			{
				CheckJumpButton();
			}

			//
			// Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
			//
			WishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
			var inSpeed = WishVelocity.Length.Clamp( 0, 1 );
			WishVelocity *= Input.Rotation;

			WishVelocity = WishVelocity.Normal * inSpeed;

			//if ( Input.Down( InputButton.Run ) )
			//{
			//	Log.Info( $"{Velocity.Length}" );
			//	WishVelocity *= WishVelocity.Normal * AirAcceleration;
			//}

			WishVelocity *= GetSpeed();

			bool bStayOnGround = false;

			ApplyFriction( 0.5f );
			AirMove();
			
			CategorizePosition( bStayOnGround );


			if ( GroundEntity != null )
			{
				//Log.Info( "1" );
				Velocity = Velocity.WithZ( 0);
			}

			SaveGroundPos();

			if ( Debug )
			{
				DebugOverlay.Box( Pos + TraceOffset, mins, maxs, Color.Red );
				DebugOverlay.Box( Pos, mins, maxs, Color.Blue );

				var lineOffset = 0;
				if ( Host.IsServer ) lineOffset = 10;

				DebugOverlay.ScreenText( lineOffset + 0, $"             Pos: {Pos}" );
				DebugOverlay.ScreenText( lineOffset + 1, $"             Vel: {Velocity}" );
				DebugOverlay.ScreenText( lineOffset + 2, $"    BaseVelocity: {BaseVelocity}" );
				DebugOverlay.ScreenText( lineOffset + 3, $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]" );
				DebugOverlay.ScreenText( lineOffset + 4, $" SurfaceFriction: {SurfaceFriction}" );
				DebugOverlay.ScreenText( lineOffset + 5, $"    WishVelocity: {WishVelocity}" );
			}

		}

		public virtual float GetSpeed()
		{
			bool leftDown = Input.Left == 1f;
			bool rightDown = Input.Left == -1f;
			bool DownDown = Input.Forward == -1f;

			Speed = Speed.Approach( 0, Speed * Time.Delta * 0.3f );

			if ( Input.Down( InputButton.Run ) & isEnergyNull == false )
			{
				Speed += 0.8f;
			}
			if ( Input.Down( InputButton.Forward ) || leftDown == true || rightDown == true || DownDown == true )
			{
				Speed += 0.5f;
			}
			if ( Speed > 400 ) Speed = 400; 
			return Speed;
		}

		/// <summary>
		/// Add our wish direction and speed onto our velocity
		/// </summary>
		public virtual void Accelerate( Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
		{
			// See if we are changing direction a bit
			var currentspeed = Velocity.Dot( wishdir );

			// Reduce wishspeed by the amount of veer.
			var addspeed = wishspeed - currentspeed;

			// If not going to add any speed, done.
			if ( addspeed <= 0 )
				return;

			// Determine amount of acceleration.
			var accelspeed = acceleration * Time.Delta * wishspeed * SurfaceFriction;
			// Cap at addspeed
			if ( accelspeed > addspeed )
				accelspeed = addspeed;

			Velocity += wishdir * accelspeed;
		}

		/// <summary>
		/// Remove ground friction from velocity
		/// </summary>
		public virtual void ApplyFriction( float frictionAmount = 1.0f )
		{
			// Calculate speed
			var speed = Velocity.Length;
			if ( speed < 0.1f ) return;

			// Bleed off some speed, but if we have less than the bleed
			//  threshold, bleed the threshold amount.
			float control = (speed < StopSpeed) ? StopSpeed : speed;

			// Add the amount to the drop amount.
			var drop = control * Time.Delta * frictionAmount;

			// scale the velocity
			float newspeed = speed - drop;
			if ( newspeed < 0 ) newspeed = 0;

			if ( newspeed != speed )
			{
				newspeed /= speed;
				Velocity *= newspeed;
			}
		}

		void CheckJumpButton()
		{
			if ( GroundEntity == null )
				return;

			ClearGroundEntity();

			float flGroundFactor = 1.0f;

			float flMul = 268.3281572999747f * 1.2f;

			float startz = Velocity.z;

			Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );

			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			AddEvent( "jump" );

		}

		void AirMove()
		{
			var wishdir = WishVelocity.Normal;
			var wishspeed = WishVelocity.Length;

			Accelerate( wishdir, wishspeed, AirControl, AirAcceleration );

			Velocity += BaseVelocity;

			//Velocity -= BaseVelocity;
		}

		Vector3[] planes = new Vector3[5];

		void CategorizePosition( bool bStayOnGround )
		{
			SurfaceFriction = 1.0f;

			// Doing this before we move may introduce a potential latency in water detection, but
			// doing it after can get us stuck on the bottom in water if the amount we move up
			// is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
			// this several times per frame, so we really need to avoid sticking to the bottom of
			// water on each call, and the converse case will correct itself if called twice.
			//CheckWater();

			var point = Pos - Vector3.Up * 2;
			var vBumpOrigin = Pos;

			//
			//  Shooting up really fast.  Definitely not on ground trimed until ladder shit
			//
			bool bMovingUpRapidly = Velocity.z > MaxNonJumpVelocity;
			bool bMovingUp = Velocity.z > 0;

			bool bMoveToEndPos = false;

			if ( GroundEntity != null ) // and not underwater
			{
				bMoveToEndPos = true;
				point.z -= StepSize;
			}
			else if ( bStayOnGround )
			{
				bMoveToEndPos = true;
				point.z -= StepSize;
			}

			var pm = TraceBBox( vBumpOrigin, point, 4.0f );

			if ( pm.Entity == null || pm.Normal.z < GroundNormalZ )
			{
				ClearGroundEntity();
				bMoveToEndPos = false;

				if ( Velocity.z > 0 )
					SurfaceFriction = 0.25f;
			}
			else
			{
				UpdateGroundEntity( pm );
			}

			if ( bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f )
			{
				Position = pm.EndPos;
			}

		}

		/// <summary>
		/// We have a new ground entity
		/// </summary>
		public virtual void UpdateGroundEntity( TraceResult tr )
		{
			GroundNormal = tr.Normal;

			// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
			// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
			// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
			SurfaceFriction = tr.Surface.Friction * 1.25f;
			if ( SurfaceFriction > 1 ) SurfaceFriction = 1;

			//if ( tr.Entity == GroundEntity ) return;

			Vector3 oldGroundVelocity = default;
			if ( GroundEntity != null ) oldGroundVelocity = GroundEntity.Velocity;

			bool wasOffGround = GroundEntity == null;

			GroundEntity = tr.Entity;

			if ( GroundEntity != null )
			{
				BaseVelocity = GroundEntity.Velocity;
			}

			/*
              	m_vecGroundUp = pm.m_vHitNormal;
	            player->m_surfaceProps = pm.m_pSurfaceProperties->GetNameHash();
	            player->m_pSurfaceData = pm.m_pSurfaceProperties;
	            const CPhysSurfaceProperties *pProp = pm.m_pSurfaceProperties;

	            const CGameSurfaceProperties *pGameProps = g_pPhysicsQuery->GetGameSurfaceproperties( pProp );
	            player->m_chTextureType = (int8)pGameProps->m_nLegacyGameMaterial;
            */
		}

		/// <summary>
		/// We're no longer on the ground, remove it
		/// </summary>
		public virtual void ClearGroundEntity()
		{
			if ( GroundEntity == null ) return;

			GroundEntity = null;
			GroundNormal = Vector3.Up;
			SurfaceFriction = 1.0f;
		}

		/// <summary>
		/// Traces the current bbox and returns the result.
		/// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
		/// position. This is good when tracing down because you won't be tracing through the ceiling above.
		/// </summary>
		public override TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
		{
			return TraceBBox( start, end, mins, maxs, liftFeet );
		}

		/// <summary>
		/// Try to keep a walking player on the ground when running down slopes etc
		/// </summary>

		void RestoreGroundPos()
		{
			if ( GroundEntity == null || GroundEntity.IsWorld )
				return;

			//var worldPos = GroundEntity.Transform.ToWorld( GroundTransform );
			//Pos = worldPos.Pos;
		}

		void SaveGroundPos()
		{
			if ( GroundEntity == null || GroundEntity.IsWorld )
				return;

			//GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Pos, Rot ) );
		}

	}
}
