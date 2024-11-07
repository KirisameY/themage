using System.Threading.Tasks;
using Godot;
using TheMage.Scripts.Interfaces;

namespace TheMage.Scripts.Weapons;

[GlobalClass]
public partial class SlimeZero : Weapon
{
	public override async void Attack(IAttackable source, Vector3? start = null, ITargetable target = null, string targetTeam = "Enemy")
	{
		if (!_canFire) return;
		_canFire = false;
		await ToSignal(source.GetTree().CreateTimer(Warmup), Timer.SignalName.Timeout);
		foreach (var node in source.GetTree().GetNodesInGroup("Player"))
		{
			if (node is not (ITargetable player and IPlayer)) return;
			player.TakeDamage(10,"Zero");
		}
		await ToSignal(source.GetTree().CreateTimer(Delay), Timer.SignalName.Timeout);
		_canFire = true;
	}
}