using Godot;

namespace TheMage.Scripts;

public partial class Game : Node3D
{
	public Player Player;
	public ProgressBar HealthBar;

	public override void _Ready()
	{
		Player = GetNode<Player>("Player");
		HealthBar = GetNode<ProgressBar>("UI/HpBar");
	}

	public override void _Process(double delta)
	{
		HealthBar.MaxValue = Player.MaxHealth;
		HealthBar.Value = Player.NowHealth;
	}
}