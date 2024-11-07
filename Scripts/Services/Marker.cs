using Godot;
using TheMage.Scripts.Interfaces;

namespace TheMage.Scripts.Services;

public partial class Marker : Node3D, ITargetable
{
	public string GetTeam()
	{
		return "Marker";
	}

	public Vector3 GetGlobalDirection()
	{
		return GlobalRotation;
	}

	public virtual void TakeDamage(float damage,string element) { }

	public void SourceFree()
	{
		QueueFree();
	}
}