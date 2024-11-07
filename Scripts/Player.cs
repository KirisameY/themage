using System;
using Godot;
using TheMage.Scripts.Interfaces;

namespace TheMage.Scripts;

public partial class Player : CharacterBody3D, IPlayer, ITargetable, IAttackable
{
	[Export] public Weapon Weapon;
	public const float Speed = 3.0f;
	public const float JumpVelocity = 4.5f;
	
	protected Sprite3D _image;
	protected Camera3D _camera;
	protected AnimationPlayer _animation;
	protected Area3D _itemPick;
	protected Vector2 _direction = Vector2.Right;
	
	public int Experience = 0;


	public float PhysicsDefence = 0;
	public float ZeroDefence = 0;
	public float EtherDefence = 0;
	public float FireDefence = 0;
	public float AirDefence = 0;
	public float WaterDefence = 0;
	public float EarthDefence = 0;
	public float MaxHealth = 200f;
	public int level = 1;
	public float NowHealth;
	
	
	protected Item _currentItem;
	
	public override void _Ready()
	{
		_image = GetNode<Sprite3D>("Image");
		_camera = GetNode<Camera3D>("Camera");
		_animation = GetNode<AnimationPlayer>("Animation");
		_itemPick = GetNode<Area3D>("ItemPick");
		AddToGroup("Player");
		NowHealth = MaxHealth;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Move(delta);
		if (Input.IsActionPressed("Attack")) Attack();
		if (Input.IsActionJustPressed("Action") && _currentItem != null && !Input.IsActionPressed("AbilityLeft") &&
		    !Input.IsActionPressed("AbilityRight")) _currentItem.OnAction();
	}

	private void Attack()
	{
		Weapon.Attack(this);
	}

	private void Move(double delta)
	{
		var velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		if (Input.IsActionJustPressed("Jump") && IsOnFloor() && !Input.IsActionPressed("AbilityLeft") &&
		    !Input.IsActionPressed("AbilityRight"))
		{
			velocity.Y = JumpVelocity;
		}
		var newRotation = Input.GetAxis("CameraLeft", "CameraRight");
		GlobalRotation += Vector3.Down * newRotation * (float)delta;
		var zoom = Input.GetAxis("CameraZoomDown", "CameraZoomUp");
		_camera.Fov = zoom switch
		{
			< 0 => MathF.Min(_camera.Fov + 0.5f, 120),
			> 0 => MathF.Max(_camera.Fov - 0.5f, 75),
			_ => _camera.Fov
		};
		var inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			_direction = new Vector2(direction.X, direction.Z);
			foreach (var body in _itemPick.GetOverlappingBodies())
			{
				if(body is not Item item) return;
				if (_currentItem == null || GlobalPosition.DistanceTo(item.GlobalPosition) <
				    _currentItem.GlobalPosition.DistanceTo(_currentItem.GlobalPosition)) _currentItem = item;
			}
			if (Input.IsActionPressed("Run"))
			{
				velocity.X = direction.X * Speed * 2;
				velocity.Z = direction.Z * Speed * 2;
				_animation.Play("Run");
			}
			else
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
				_animation.Play("Move");
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			_animation.Play("Idle");
		}
		_image.FlipH = inputDir.X switch
		{
			> 0 => false,
			< 0 => true,
			_ => _image.FlipH
		};
		Velocity = velocity;
		MoveAndSlide();
		
		if(GlobalPosition.Y < -10) GlobalPosition = new Vector3(0, 1, 0);
	}

	public string GetTeam()
	{
		return "Player";
	}

	public new Vector3 GetGlobalPosition()
	{
		return GlobalPosition;
	}

	public Vector3 GetGlobalDirection()
	{
		return GlobalRotation;
	}

	public Node3D GetObjects()
	{
		return GetNode<Node3D>("%GameObjects");
	}

	public Vector2 GetDirection()
	{
		return _direction;
	}

	public virtual void TakeDamage(float damage, string element)
	{
		NowHealth -= damage * (1 - element switch
		{
			"Fire" => FireDefence,
			"Air" => AirDefence,
			"Water" => WaterDefence,
			"Earth" => EarthDefence,
			"Zero" => ZeroDefence,
			"Ether" => EtherDefence,
			_ => PhysicsDefence
		});
	}

	public void SourceFree() { }

	public void OnItemPicked(Node3D body)
	{
		if(body is not Item item) return;
		item.OnCloseToPlayer();
	}

	public void OnItemUnpicked(Node3D body)
	{
		if(body is not Item item) return;
		item.OnFarFromPlayer();
		if(_currentItem == item) _currentItem = null;
	}
	
	public override void _Input(InputEvent @event)
	{
		switch (@event)
		{
			case InputEventMouseMotion motion:
				GlobalRotation += Vector3.Down * motion.Relative.X / (Engine.GetPhysicsTicksPerSecond() * 8f);
				break;
			case InputEventMouseButton mouseButton:
				var zoom = mouseButton.ButtonIndex switch
				{
					MouseButton.WheelDown => -1,
					MouseButton.WheelUp => 1,
					_ => 0
				};
				_camera.Fov = zoom switch
				{
					< 0 => MathF.Min(_camera.Fov + 1.5f, 120),
					> 0 => MathF.Max(_camera.Fov - 1.5f, 75),
					_ => _camera.Fov
				};
				break;
		}
	}
}