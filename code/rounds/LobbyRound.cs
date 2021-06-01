using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallsWars
{
	partial class LobbyRound : BaseRound
	{
		[Net]
		public int playerCount { get; set; }

		public override string Name => "Lobby";

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );
			CheckReady();
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );
			CheckReady();
		}

		public override void Start()
		{
			Log.Info( "Start Lobby" );
			CheckReady();
		}

		public override void OnTick()
		{

		}

		private void CheckReady(Player ignored = null)
		{
			if (Host.IsClient) return;

			if ( BallGame.Instance.GetPlayerCount() >= 2)
			{
				Log.Info("True");
				foreach ( Player player in Client.All )
				{
					(player as BallPlayer).Spectator = false;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new MainRound() );
			}
		}
	}
}
