using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using TheMage.Scripts.Interfaces;
using TheMage.Scripts.Services;

namespace TheMage.Scripts;

[GlobalClass]
public partial class Weapon : Resource
{
	[Export] public PackedScene BulletScene;
	[Export] public float Warmup;
	[Export] public float Delay;
	[Export] public float Range;
	[Export] public int Level;
	[Export] public bool IsTargetGround;
	[Export] public Texture2D Texture;
	[Export] public Dictionary<string, float> Data = new();
	
	protected bool _canFire = true;

	public virtual async void Attack(IAttackable source, Vector3? start = null, ITargetable target = null,
		string targetTeam = "Enemy")

	{
		if (!_canFire) return;
		_canFire = false;
		await ToSignal(source.GetTree().CreateTimer(Warmup), Timer.SignalName.Timeout);
		var bullet = BulletScene.Instantiate<Bullet>();
		bullet.Position = start ?? source.GetGlobalPosition();
		bullet.Target = target;
		bullet.TargetTeam = targetTeam;
		bullet.Rotation = new Vector3(0, source.GetDirection().Angle(), 0);
		source.GetObjects().AddChild(bullet);
		await ToSignal(source.GetTree().CreateTimer(Delay), Timer.SignalName.Timeout);
		_canFire = true;
	}
}