using Godot;
using System;

public partial class Enemys : CharacterBody2D
{
	[Export] public int MaxHp = 1;
	[Export] public float Speed = 80f;
	[Export] public float AttackRange = 125f;
	[Export] public float AttackCooldown = 1.2f;
	[Export] public int Damage = 1;

	private int _hp;
	private bool _dead;
	private float _cooldown;
	private float _attackTimer;
	private bool _attacking;
	private int _facing = 1;

	private Node2D _player;

	public override void _Ready()
	{
		_hp = MaxHp;
		AddToGroup("enemy");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_dead) return;

		if (_player == null || !IsInstanceValid(_player) || !_player.IsInGroup("player"))
			_player = GetTree().GetFirstNodeInGroup("player") as Node2D;

		Vector2 velocity = Vector2.Zero;

		if (_attacking)
		{
			// Ütés közben nem mozog és nem fordul
			velocity = Vector2.Zero;
		}
		else if (_player != null)
		{
			Vector2 vektor = _player.GlobalPosition - GlobalPosition;
			float distSq = vektor.LengthSquared();
			float rangeSq = AttackRange * AttackRange;

			if (vektor.X != 0)
				_facing = vektor.X > 0 ? 1 : -1;

			// Csak akkor közelít, ha még nincs ütőtávon belül
			if (distSq > rangeSq)
			{
				velocity = vektor.Normalized() * Speed;
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
