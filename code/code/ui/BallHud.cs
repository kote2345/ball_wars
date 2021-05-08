using Sandbox;
using Sandbox.UI;

namespace BallsWars
{
	[Library]
	public partial class BallHud : Hud
	{
		public BallHud()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/BallHud.scss" );

			RootPanel.AddChild<BallNameTags>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<BallEnergy>();
			RootPanel.AddChild<BallWeight>();
			RootPanel.AddChild<WaitingPeople>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		}
	}
}
