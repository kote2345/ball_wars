
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;


namespace BallsWars
{
	[Library( "balls_wars" )]
	public partial class BallGame : Sandbox.Game
	{
		[Net] 
		public BaseRound Round { get; set; }

		[Net]
		public int PlayerCount { get; set; }
		[Net]
		public Player WinPlayer { get; set; }
		private BaseRound _lastRound;

		public bool Spectator = true;

		public BallGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );
				new BallHud();

			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
			}
			_ = StartSecondTimer();
			ChangeRound(new LobbyRound());
		}

		public static BallGame Instance
		{
			get => Current as BallGame;
		}

		public async Task StartSecondTimer()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				Round?.OnTick();
			}
		}
		public int GetPlayerCount()
		{
			if ( IsServer )
			{
				PlayerCount = Player.All.Count;
			}
			//Log.Info( "GetPlayerCount" );
			//Log.Info( $"{PlayerCount}" );
			return PlayerCount;
		}
		public void ChangeRound(BaseRound round)
		{
			Assert.NotNull(round);

			Round?.Finish();
			Round = round;
			Round?.Start();
		}

		public override Player CreatePlayer() => new BallPlayer();
		public override void PlayerDisconnected( Sandbox.Player player, NetworkDisconnectionReason reason )
		{
			base.PlayerDisconnected( player, reason );
			Round?.PlayerDisconnected( player, reason );
			Log.Info( "Disconnected" );
		}
		public override void PlayerJoined( Player player )
		{
			base.PlayerJoined( player );
			Log.Info( "Connected" );
			Round?.PlayerJoined( player );
		}
		public override void PlayerKilled( Player player )
		{
			base.PlayerKilled( player );
			Round?.PlayerKilled( player );
		}
		private void OnTick()
		{
			Round?.OnTick();

			// From: https://github.com/Facepunch/sbox-hidden/blob/71e32f7d1f28d51a5c3078e99f069d0b73ef490d/code/Game.cs#L186
			if ( IsClient )
			{
				// We have to hack around this for now until we can detect changes in net variables.
				if ( _lastRound != Round )
				{
					_lastRound?.Finish();
					_lastRound = Round;
					_lastRound.Start();
				}
			}
		}
	}
}
