using Godot;

namespace TheMage.Scripts.Interfaces;

public interface ITargetable
{
	public string GetTeam();
	public Vector3 GetGlobalPosition();
	public Vector3 GetGlobalDirection();
	public SceneTree GetTree();
	public Node GetNode(NodePath path);
	
	public void TakeDamage(float damage,string element);
	public void SourceFree();
}