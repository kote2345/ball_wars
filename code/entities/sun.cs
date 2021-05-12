using Sandbox;

[Library( "ent_sun" )]
public partial class LampEntity : SunLight
{
	public LampEntity()
	{
		this.WorldPos = Vector3.Zero;
	}
}
