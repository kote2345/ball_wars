using System.Collections.Generic;
using Sandbox;

namespace BallsWars
{
    public partial class BaseRound : NetworkClass
	{
		[Net] public TimeSince TimeElapsed { get; set; }
		public virtual int RoundLength { get; set; }
		public virtual string Name => "Base Round";

		public BaseRound()
		{
			TimeElapsed = 0;
		}
		public float RoundEndTime { get; set; }

		public float TimeLeft
		{
			get
			{
				return RoundEndTime - Sandbox.Time.Now;
			}
		}
		public virtual void Start() { }
		public virtual void Finish() { }
		public virtual void OnTick() { }
		public virtual void OnKilled() { }
		public virtual void ClientJoined( Client cl ) { }
		public virtual void ClientDisconnect( Client cl, NetworkDisconnectionReason reason ) { }
	}
}
