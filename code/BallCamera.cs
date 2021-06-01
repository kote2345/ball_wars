using Sandbox;

public class BallCamera : Camera
{
	private Vector3 focusPoint;

	public override void Activated()
	{
		base.Activated();

		focusPoint = GetSpectatePoint();
		Pos = focusPoint + GetViewOffset();
		FieldOfView = CurrentView.FieldOfView;
	}

	public override void Update()
	{
		var player = Local.Client;
		if ( player == null ) return;

		focusPoint = GetSpectatePoint();
		Pos = focusPoint + GetViewOffset();

		var tr = Trace.Ray( focusPoint, Pos )
			.Ignore( Local.Pawn )
			.WorldOnly()
			.Radius( 4 )
			.Run();

		//
		// Doing a second trace at the half way point is a little trick to allow a larger camera collision radius
		// without getting initially stuck
		//
		tr = Trace.Ray( focusPoint + tr.Direction * (tr.Distance * 0.5f), tr.EndPos )
			.Ignore( Local.Pawn )
			.WorldOnly()
			.Radius( 8 )
			.Run();

		Pos = tr.EndPos;
		Rot = Local.Pawn.EyeRot;

		Viewer = null;

	}

	public virtual Vector3 GetSpectatePoint()
	{
		if ( Local.Pawn is not Player player )
			return Local.Pawn.Position;

		if ( !player.Corpse.IsValid() || player.Corpse is not ModelEntity corpse )
			return player.GetBoneTransform( player.GetBoneIndex( "spine2" ) ).Pos;

		return corpse.GetBoneTransform( corpse.GetBoneIndex( "spine2" ) ).Pos;
	}

	public virtual Vector3 GetViewOffset()
	{
		if ( Local.Pawn is not Player player ) return Vector3.Zero;

		return player.EyeRot.Forward * -200 + Vector3.Up * 20;
	}
}
