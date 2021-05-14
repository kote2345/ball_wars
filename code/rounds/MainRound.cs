using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallsWars
{
	public class MainRound : BaseRound
	{
		int timer = 10;
		public int PlayersAlive = 0;
		public override string Name => "MainRound";

		public override void PlayerJoined(Player player)
		{
			base.PlayerJoined(player);
			CheckPlayer();
		}

		public override void PlayerDisconnected(Player player, NetworkDisconnectionReason reason)
		{
			base.PlayerDisconnected(player, reason);
			CheckPlayer();
		}

		public override void OnTick()
		{
			CheckPlayer();
		}
		public override void PlayerKilled( Player player ) 
		{
			Log.Info("Killed");
			//(player as BallPlayer).Spectator = true;
			CheckPlayer();
			base.PlayerKilled( player );
		}
		private void CheckPlayer( Player ignored = null )
		{
			if ( Host.IsClient ) return;

			if ( BallGame.Instance.GetPlayerCount() < 2)
			{
				List<Player> allPlayers2 = Player.All.Where( player => player != ignored ).ToList();

				foreach ( Player player in allPlayers2 )
				{
					(player as BallPlayer).Spectator = true;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new LobbyRound() );
			}

			List<Player> allPlayers = Player.All.Where( player => player != ignored ).ToList();

			PlayersAlive = 0;

			foreach ( Player player in allPlayers )
			{
				if ( (player as BallPlayer).Spectator == false )
				{
					Log.Info( "Check Player" );
					PlayersAlive++;
				}
			}
			if ( PlayersAlive == 1 )
			{
				Log.Info( "End" );
				BallGame.Instance.ChangeRound( new EndRound() );
			}
		}
	}
}
