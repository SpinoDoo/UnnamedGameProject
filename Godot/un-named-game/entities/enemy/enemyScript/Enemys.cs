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

        float dt = (float)delta;
        if (_cooldown > 0f)
            _cooldown -= dt;

        if (_player == null || !IsInstanceValid(_player) || !_player.IsInGroup("player"))
            _player = GetTree().GetFirstNodeInGroup("player") as Node2D;

        Vector2 velocity = Vector2.Zero;

        if (_player != null)
        {
            Vector2 vektor = _player.GlobalPosition - GlobalPosition;
            float distSq = vektor.LengthSquared();
            float rangeSq = AttackRange * AttackRange;

            if (vektor.X != 0)
                _facing = vektor.X > 0 ? 1 : -1;

            if (distSq > rangeSq)
            {
                velocity = vektor.Normalized() * Speed;
            }
            else
            {
                velocity = Vector2.Zero;
                if (_cooldown <= 0f)
                {
                    _cooldown = AttackCooldown;
                    DealDamage();
                }
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void DealDamage()
    {
        if (_dead || _player == null || !IsInstanceValid(_player)) return;

        Vector2 d = _player.GlobalPosition - GlobalPosition;
        if (d.LengthSquared() <= AttackRange * AttackRange && _player is Mc mc)
        {
            mc.TakeDamage(Damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_dead) return;

        _hp -= damage;

        if (_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _dead = true;
        RemoveFromGroup("enemy");

        var shape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        shape?.SetDeferred("disabled", true);

        QueueFree();
    }
}