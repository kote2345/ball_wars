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
					BallGame.Instance.ChangeRound( new LobbyRound() );
				}
			}

			base.OnTick();
		}

		private void CheckPlayer( Player ignored = null )
		{
			if ( BallGame.Instance.GetPlayerCount() < 2 )
			{
				Entity.All.Where( player => player is Player ).Cast<Player>().ToList();
				//List<Player> allPlayers = Entity.All.Where( player => player != ignored ).ToList();
				var PlayerList = Client.All;
				foreach ( Player player in Client.All )
				{
					(player as BallPlayer).Spectator = true;
					player.Respawn();
				}
				BallGame.Instance.ChangeRound( new LobbyRound() );
			}
		}
	}
}
