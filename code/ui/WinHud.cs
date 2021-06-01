
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace BallsWars
{
	public class WinHud : Panel
	{
		public Label NameLabel;
		public Image Avatar;

		public WinHud()
		{
			NameLabel = Add.Label( "Win" );
			//Avatar = Add.Image( $"avatar:{player.SteamId}" );
		}

		public override void Tick()
		{
			//Log.Info( $"{BallGame.Instance.GetPlayerCount()}" );
			if ( BallGame.Instance.Round is EndRound )
			{
				NameLabel.SetText( BallGame.Instance.WinPlayer.EntityName + "\nWin !");
			}
			else
			{
				NameLabel.SetText( "" );
			}
		}
	}
}
