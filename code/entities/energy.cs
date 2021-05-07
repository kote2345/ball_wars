using Sandbox;
using System.Threading.Tasks;

namespace BallsWars
{
	[Library( "ent_energy" )]
	public partial class EnergyEntity : Prop
	{
		public float rotation = 0;
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "models/energy.vmdl" );
			ChangeColor();
			ChangeRotation();
			EnableTouch = true;
		}
		public void CheckPlayers()
		{
			Sphere sphere = new Sphere();
			sphere.Radius = 100;
			sphere.Trace( new Ray( WorldPos, Vector3.Up ), 200 );
		}

		public async Task ChangeColor()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				RenderColor = Color.Random.ToColor32();
			}
		}
		public async Task ChangeRotation()
		{
			while ( true )
			{
				await Task.DelaySeconds( 0.001f );
				rotation += 1;
				LocalRot = Rotation.FromYaw( rotation );
			}
		}
		public override void Touch( Entity other )
		{
			Log.Info( "Energy Touch" );
			base.Touch( other );
		}
	}
}
