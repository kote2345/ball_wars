using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace BallsWars
{
	public partial class EndRound : BaseRound
	{
		public override string Name => "End Round";
		public override int RoundLength => 8;

		public override void OnTick()
		{
			CheckPlayer();
			if ( Host.IsServer )
			{
				if ( TimeElapsed > RoundLength )
				{
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
