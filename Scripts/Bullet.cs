using Godot;
using TheMage.Scripts.Interfaces;
using TheMage.Scripts.Services;

namespace TheMage.Scripts;

public partial class Bullet : Area3D
{
	public ITargetable Target;
	public string TargetTeam;
	[Export] public string Element = "Physics"; 
	[Export] public float DirectDamage;
	[Export] public float AreaDamage;
	[Export] public float AreaRadius;
	[Export] public float BulletSpeed;
	[Export] public float LifeTime;
	[Export] public bool CanExplode;
	[Export] public bool CanTarget;
	[Export] public float FindRadius;
	[Export] public bool MoreTargets;

	protected void BaseReady()
	{
		if (Target == null)
		{
			foreach (var node in GetTree().GetNodesInGroup(TargetTeam))
			{
				if (node is not ITargetable target) continue;
				if(GlobalPosition.DistanceTo(target.GetGlobalPosition()) > FindRadius) continue;
				if (Target == null)
				{
					if (Target == null)
					{
						var newTarget = new Marker();
						newTarget.Position = GlobalPosition + new Vector3(Mathf.Cos(GlobalRotation.Y), 0, Mathf.Sin(GlobalRotation.Y)) *
						                     BulletSpeed * LifeTime;
						Game.GetNode("%GameObjects").AddChild(newTarget);
						Target = target;
					}
				}
				if (GlobalPosition.DistanceTo(target.GetGlobalPosition()) <
				    GlobalPosition.DistanceTo(Target.GetGlobalPosition())) Target = target;
			}

			if (Target == null)
			{
				var target = new Marker();
				target.Position =GlobalPosition + new Vector3(Mathf.Cos(GlobalRotation.Y), 0, Mathf.Sin(GlobalRotation.Y)) *
				                  BulletSpeed * LifeTime;
				Game.GetNode("%GameObjects").AddChild(target);
				Target = target;
			}
		}

		var tween = GetTree().CreateTween();
		tween.TweenProperty(this, "global_position", GlobalPosition, 0);
		tween.TweenProperty(this, "global_position", Target.GetGlobalPosition(),
			GlobalPosition.DistanceTo(Target.GetGlobalPosition()) / BulletSpeed);
		tween.TweenCallback(Callable.From(Target.SourceFree));
		tween.TweenCallback(Callable.From(Explore));
		tween.TweenCallback(Callable.From(QueueFree));
	}
	public override void _Ready()
	{
		BaseReady();
	}

	public Game Game => GetTree().Root.GetNode<Game>("Game");

	protected void Explore()
	{
		if (!CanExplode) return;
		AreaAttack();
	}

	private void AreaAttack()
	{
		if(AreaRadius == 0) return;
		foreach (var node in GetTree().GetNodesInGroup(TargetTeam))
		{
			if (node is not ITargetable target) continue;
			var distance = GlobalPosition.DistanceTo(target.GetGlobalPosition());
			if(distance > AreaRadius) continue;
			target.TakeDamage(AreaDamage * distance / AreaRadius, Element);
		}
	}

	private void DirectAttack(ITargetable target)
	{
		target.TakeDamage(DirectDamage, Element);
	}

	public void Attack(Node3D body)
	{
		switch (body)
		{
			case ITargetable target:
				DirectAttack(target);
				AreaAttack();
				break;
			default:
				Explore();
				break;
		}
		if (MoreTargets) return;
		Target?.SourceFree();
		QueueFree();
	}
}