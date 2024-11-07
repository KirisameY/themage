using Godot;

namespace TheMage.Scripts;

public partial class Item : CharacterBody3D
{
	public virtual void OnCloseToPlayer(){}
	public virtual void OnAction(){}
	public virtual void OnFarFromPlayer(){}
	public virtual void OnHit(){}
}