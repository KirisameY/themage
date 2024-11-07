using Godot;
using TheMage.Scripts.Weapons;

namespace TheMage.Scripts.Items;

public partial class SpawnerDebug : Item
{
	protected PackedScene _enemyScreen = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/SlimeZero.tscn");
	protected Enemy _newEnemy => _enemyScreen.Instantiate<Enemy>();

	public override void OnAction()
	{
		var newEnemy = _newEnemy;
		newEnemy.Position = GlobalPosition + new Vector3(1, 1, 1);
		GetTree().Root.GetNode<Node3D>("Game/GameObjects").AddChild(newEnemy);
	}
}
