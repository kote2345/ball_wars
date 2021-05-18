using Sandbox;
using System;
using System.Linq;

namespace BallsWars
{
	partial class BallPlayer : BasePlayer
	{
		[Net]
		public bool Spectator { get; set; }
		[Net]
		public int Energy { get; set; }
		public override void Spawn()
		{
			Spectator = true;
		}
		public override void Respawn()
		{
			Energy = 100;
			if ( Spectator == true)
			{
				Controller = new NoclipController();

				Camera = new FirstPersonCamera();

				EnableAllCollisions = false;
				EnableDrawing = false;
				EnableHideInFirstPerson = false;
				EnableShadowInFirstPerson = true;
			}
			else
			{
				SetModel("models/ball_player.vmdl");
				Controller = new BallController();

				Camera = new BallCamera();

				EnableAllCollisions = true;
				EnableDrawing = true;
				EnableHideInFirstPerson = true;
				EnableShadowInFirstPerson = true;
			}

			base.Respawn();

			if ( Spectator == false) SetupPhysicsFromModel(PhysicsMotionType.Dynamic, true);
		}
		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );
			//var ent = other as Model;
			//other.Delete();
		}


		protected override void Tick()
		{
			if ( Input.Down( InputButton.Run ) & Energy > 1 )
			{
				Energy--;
				Log.Info( $"{Energy}" );
			}
			if ( Controller is BallController )
			{
				if ( Energy < 1)
				{
					(Controller as BallController).isEnergyNull = true;
				}
				else
				{
					(Controller as BallController).isEnergyNull = false;
				}
			}
			base.Tick();
		}

		public override void OnKilled()
		{
			Spectator = true;
			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
			base.OnKilled();
		}

		//public void OnPostPhysicsStep(float dt)
		//{
		//	if (!this.IsValid())
		//		return;

		//	var body = PhysicsBody;
		//	if (!body.IsValid())
		//		return;
		//}
	}
}
