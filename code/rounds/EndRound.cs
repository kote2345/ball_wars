using Sandbox;

namespace BallsWars
{
	public partial class EndRound : BaseRound
	{
		public override string Name => "End Round";
		public override int RoundLength => 8;

		public override void OnTick()
		{
			if ( Host.IsServer )
			{
				if ( TimeElapsed > RoundLength )
				{
					BallGame.Instance.ChangeRound( new LobbyRound() );
				}
			}

			base.OnTick();
		}
	}
}
