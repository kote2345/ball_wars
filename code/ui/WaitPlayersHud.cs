using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

	public class WaitingPeople : Panel
	{
		public Label Text;

		public WaitingPeople()
		{
			Text = Add.Label("Waiting People");
		}

		public override void Tick()
		{			
			if (Player.All.Count == 2)
			{
				Text.SetText( " " );
			}
			else
			{
				Text.SetText( "Waiting People" );
			}	
		}
	}
