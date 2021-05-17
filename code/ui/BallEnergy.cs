using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace BallsWars
{
	public partial class BallEnergy : Panel
	{
		private Panel PowerBar;

		public BallEnergy()
		{
			Add.Label("⚡");
			PowerBar = Add.Panel("bar").Add.Panel();
	}

		public override void Tick()
		{
			var player = Player.Local as BallPlayer;
			if ( player == null ) return;
			PowerBar.Style.Width = player.Energy;
			PowerBar.Style.Dirty();

			SetClass( "spectator", player.Spectator == true );
		}
	}
}
