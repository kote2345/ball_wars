using Sandbox;
using System.Threading.Tasks;

namespace BallsWars
{
	[Library( "ent_energy" )]
	public partial class EnergyEntity : Prop
	{
		public bool alive = true;
		public float rotation = 0;
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/energy_ent.vmdl" );
			ChangeColor();
			ChangeRotation();
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			CollisionGroup = CollisionGroup.Trigger;
			EnableTouch = true;
		}
		public void CheckPlayers()
		{
			var tr = Trace.Ray( WorldPos, WorldPos + new Vector3( 0, 0, 200 ) )
				.EntitiesOnly()
				.Size( 100 )
				.Run();
			if ( tr.Hit == true ) Log.Info( "Hit" );
		}

		public async Task ChangeColor()
		{
			while ( alive )
			{
				await Task.DelaySeconds( 1 );
				RenderColor = Color.Random.ToColor32();
			}
		}
		public async Task ChangeRotation()
		{
			while ( alive )
			{
				await Task.DelaySeconds( 0.001f );
				rotation += 1;
				LocalRot = Rotation.FromYaw( rotation );
			}
		}
		public override void StartTouch( Entity other )
		{
			alive = false;
			Log.Info( "Energy Touch" );
			base.Touch( other );
			if ( other is BallPlayer )
			{
				if ( (other as BallPlayer).Energy == 400 ) return;
				if ( (other as BallPlayer).Energy >= 350 )
				{
					(other as BallPlayer).Energy = 400;
				}
				else
				{
					(other as BallPlayer).Energy += 50;
				}
				if ( IsServer ) Delete();
			}
		}
	}
}
