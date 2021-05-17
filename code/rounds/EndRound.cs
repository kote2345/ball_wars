using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace BallsWars
{
	public partial class EndRound : BaseRound
	{
		private WinHud _statsPanel;
		public override string Name => "End Round";
		public override int RoundLength => 8;

		public override void Start()
		{
			Log.Info( "Start End Round" );
			if ( Host.IsClient )
			{
				_statsPanel = Sandbox.Hud.CurrentPanel.AddChild<WinHud>();
			}
		}

		public override void Finish()
		{
			Log.Info( "Finished Stats Round" );
		}

		public override void OnTick()
		{
			CheckPlayer();
			if ( Host.IsServer )
			{
				if ( TimeElapsed > RoundLength )
				{
					if ( _statsPanel != null )
					{
						Log.Info( "Kill HUD" );
						_statsPanel.Delete();
					}
					BallGame.Instance.ChangeRound( new LobbyRound() );
				}
			}

			base.OnTick();
		}

		private void CheckPlayer( Player ignored = null )
		{
			if ( BallGame.Instance.GetPlayerCount() < 2 )
			{
				List<Player> allPlayers = Player.All.Where( player => player != ignored ).ToList();

				foreach ( Player player in allPlayers )
				{
					(player as BallPlayer).Spectator = true;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new LobbyRound() );
			}
		}
	}
}
