using Godot;

namespace TheMage.Scripts.Items;

public partial class Board : Item
{
	protected Label3D _text;
	[Export] public string LabelText;

	public override void _Ready()
	{
		_text = GetNode<Label3D>("Text");
		_text.Text = LabelText;
		_text.Hide();
	}

	public override void OnCloseToPlayer()
	{
		_text.Show();
	}

	public override void OnFarFromPlayer()
	{
		_text.Hide();
	}
}