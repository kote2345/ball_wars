using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace BallsWars
{
	public partial class BallWeight : Panel
	{
		private Label Weight;

		public BallWeight()
		{
			Add.Label( "Weight: " );
			Weight = Add.Label( "5", "value" );
		}

		public override void Tick()
		{
			if ( Sandbox.Player.Local is not Player player ) return;
			SetClass( "spectator", (player as BallPlayer).Spectator == true );
		}
	}
}
