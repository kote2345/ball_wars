using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace BallsWars
{
	public class WaitingPeople : Panel
	{
		public int PlayerCount;
		public Label Text;

		public WaitingPeople()
		{
			Text = Add.Label( "Waiting People" );
		}

		public override void Tick()
		{
			//Log.Info( $"{BallGame.Instance.GetPlayerCount()}" );
			if ( BallGame.Instance.GetPlayerCount() >= 2 )
			{
				Text.SetText( " " );
			}
			else
			{
				Text.SetText( "Waiting People" );
			}
		}
	}
}
