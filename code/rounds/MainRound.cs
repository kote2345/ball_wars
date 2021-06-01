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

		public override void ClientJoined(Client cl)
		{
			base.ClientJoined( cl );
			CheckPlayer();
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason)
		{
			base.ClientDisconnect( cl, reason);
			CheckPlayer();
		}

		public override void OnTick()
		{
			CheckPlayer();
		}
		public override void OnKilled() 
		{
			Log.Info("Killed");
			//(player as BallPlayer).Spectator = true;
			CheckPlayer();
			base.OnKilled();
		}
		private void CheckPlayer( Player ignored = null )
		{
			if ( Host.IsClient ) return;

			if ( BallGame.Instance.GetPlayerCount() < 2)
			{
				foreach ( Player player in Client.All )
				{
					(player as BallPlayer).Spectator = true;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new LobbyRound() );
			}

			PlayersAlive = 0;

			//foreach ( Player player in Client.All )
			//{
			//	if ( (player as BallPlayer).Spectator == false )
			//	{
					//Log.Info( "Check Player" );
			//		PlayersAlive++;
			//		BallGame.Instance.WinPlayer = player;
			//	}
			//}

			if ( PlayersAlive == 1 )
			{
				Log.Info( "End" );
				BallGame.Instance.ChangeRound( new EndRound() );
			}
		}
	}
}
