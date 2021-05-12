using Sandbox;
using System;
using System.Linq;

namespace BallsWars
{
	partial class BallPlayer : BasePlayer
	{
		[Net]
		public bool Spectator { get; set; }
		
		public override void Spawn()
		{
			Spectator = true;
		}
		public override void Respawn()
		{
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
				SetModel("models/ball.vmdl");
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
			Log.Info( "Touch" );
			//var ent = other as Model;
			//other.Delete();
		}


		protected override void Tick()
		{
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
