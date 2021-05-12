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
			if ( Sandbox.Player.Local is not Player player ) return;
			SetClass( "spectator", (player as BallPlayer).Spectator == true );
			PowerBar.Style.Width = 100;
			PowerBar.Style.Dirty();
		}
	}
}
