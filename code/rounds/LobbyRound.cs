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

		public override void PlayerJoined(Player player)
		{
			Log.Info("Player Joined");
			base.PlayerJoined(player);
			Log.Info("Joined");
			CheckReady();
			Log.Info("Player Joined");
		}

		public override void PlayerDisconnected(Player player, NetworkDisconnectionReason reason)
		{
			base.PlayerDisconnected(player, reason);
			CheckReady(player);
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

			int playerCount = Player.All.Count;
			if (playerCount >= 2)
			{
				Log.Info("True");
				List<Player> allPlayers = Player.All.Where(player => player != ignored).ToList();

				foreach (Player player in allPlayers)
				{
					(player as BallPlayer).Spectator = false;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new MainRound() );
			}
		}
	}
}
