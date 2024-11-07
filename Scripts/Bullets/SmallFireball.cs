using Godot;

namespace TheMage.Scripts.Bullets;

public partial class SmallFireball : Bullet
{
	private GpuParticles3D _tail;

	public override void _Ready()
	{
		BaseReady();
		_tail = GetNode<GpuParticles3D>("Tail");
		RemoveChild(_tail);
		_tail.Rotation = GlobalRotation;
		GetParent().AddChild(_tail);
	}

	public override void _PhysicsProcess(double delta)
	{
		_tail.GlobalPosition = GlobalPosition;
	}
}