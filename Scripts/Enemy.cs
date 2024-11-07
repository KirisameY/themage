using Godot;
using TheMage.Scripts.Interfaces;

namespace TheMage.Scripts;

public partial class Enemy : CharacterBody3D, ITargetable, IAttackable
{
	[Export] public Weapon Weapon;
	[Export] public float Speed = 1.0f;
	[Export] public float JumpVelocity = 5f;
	[Export] public float ViewRange = 5f;
	[Export] public float AlertRange = 20f;
	[Export] public float AttackRange  = 1f;
	[Export] public float MaxHealth = 100f;
	[Export] public int level = 1;

	[Export(PropertyHint.Range,"-1,1")] public float PhysicsDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float ZeroDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float EtherDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float FireDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float AirDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float WaterDefence = 0;
	[Export(PropertyHint.Range,"-1,1")] public float EarthDefence = 0;
	[Export] public int Exp;
	
	[Export] public string AnimationId;
	
	protected float _nowHealth;
	protected IPlayer _target;

	protected NavigationAgent3D _agent;
	protected AnimationPlayer _animation;
	protected RayCast3D _ray;
	protected Sprite3D _sprite;
	protected MeshInstance3D _hpMesh;

	public override void _Ready()
	{
		AddToGroup("Enemy");
		_nowHealth = MaxHealth;
		_agent = GetNode<NavigationAgent3D>("Agent");
		_animation = GetNode<AnimationPlayer>("Animation");
		_ray = GetNode<RayCast3D>("Ray");
		_hpMesh = GetNode<MeshInstance3D>("HpBar");
		_sprite = GetNode<Sprite3D>("Image");
	}

	public override void _Process(double delta)
	{
		var shader = (ShaderMaterial)_hpMesh.Mesh.SurfaceGetMaterial(0);
		shader.SetShaderParameter("hp",_nowHealth/MaxHealth);
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		
		
		foreach (var node in GetTree().GetNodesInGroup("Player"))
		{
			if (node is not IPlayer player) continue;
			if (_target == null)
			{
				if (GlobalPosition.DistanceTo(player.GetGlobalPosition()) < ViewRange) _target = player;
			}
			else if (GlobalPosition.DistanceTo(player.GetGlobalPosition()) <
			         GlobalPosition.DistanceTo(_target.GetGlobalPosition())) _target = player;
		}
		if(_target != null && GlobalPosition.DistanceTo(_target.GetGlobalPosition()) > AlertRange)
			_target = null;
		if (_target != null && GlobalPosition.DistanceTo(_target.GetGlobalPosition()) > AttackRange)
		{
			_agent.TargetPosition = _target.GetGlobalPosition();
			var rotation = ToLocal(_agent.GetNextPathPosition()).Normalized();
			velocity.X = rotation.X * Speed;
			velocity.Z = rotation.Z * Speed;
			_animation.Play($"{AnimationId}/Move");
			_sprite.FlipH = _target.ToLocal(GlobalPosition).X switch
			{
				< 0 => false,
				> 0 => true,
				_ => _sprite.FlipH
			};
		}
		else
		{
			velocity.X = velocity.Z = 0;
			if (_target != null)
			{
				Weapon.Attack(this, targetTeam: "Player");
				_animation.Play($"{AnimationId}/Attack");
			}
			else
			{
				
				_animation.Play($"{AnimationId}/Idle");
			}
		}
		
		
		_ray.TargetPosition = new Vector3(velocity.Normalized().X,0,velocity.Normalized().Z) *　0.6f;
		if (_ray.IsColliding() && IsOnFloor()) velocity.Y = JumpVelocity;
		Velocity = velocity;
		MoveAndSlide();
	}

	public string GetTeam()
	{
		return "Enemy";
	}
	
	public new Vector3 GetGlobalPosition()
	{
		return IsInsideTree() ? GlobalPosition : Position;
	}

	public Vector3 GetGlobalDirection()
	{
		return IsInsideTree() ? GlobalRotation : Rotation;
	}

	public Node3D GetObjects()
	{
		return GetTree().Root.GetNode<Game>("Game").GetNode<Node3D>("GameObjects");
	}

	public Vector2 GetDirection()
	{
		return new Vector2(GlobalRotation.X, GlobalRotation.Z);
	}

	public virtual void TakeDamage(float damage,string element)
	{
		if(_target == null)	foreach (var node in GetTree().GetNodesInGroup("Player"))
		{
			if (node is not IPlayer player) continue;
			if (_target == null)
			{
				if (GlobalPosition.DistanceTo(player.GetGlobalPosition()) < AlertRange) _target = player;
			}
			else if (GlobalPosition.DistanceTo(player.GetGlobalPosition()) <
			         GlobalPosition.DistanceTo(_target.GetGlobalPosition())) _target = player;
		}

		_nowHealth -= damage * (1 - element switch
		{
			"Fire" => FireDefence,
			"Air" => AirDefence,
			"Water" => WaterDefence,
			"Earth" => EarthDefence,
			"Zero" => ZeroDefence,
			"Ether" => EtherDefence,
			_ => PhysicsDefence
		});
		if (_nowHealth <= 0) Death();
	}

	private void Death()
	{
		GetTree().Root.GetNode<Player>("Game/Player").Experience += Exp;
		QueueFree();
	}

	public void SourceFree() { }
}