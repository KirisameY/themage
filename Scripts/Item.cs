using Godot;

namespace TheMage.Scripts;

public partial class Item : CharacterBody3D
{
	public virtual void OnCloseToPlayer(){}
	public virtual void OnAction(){}
	public virtual void OnFarFromPlayer(){}
	public virtual void OnHit(){}

	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Root.GetNode<Player>("Game/Player").CurrentItem == this)
		{
			GetNode<Node3D>("Info").Show();
		}
		else
		{
			GetNode<Node3D>("Info").Hide();
		}
	}
}