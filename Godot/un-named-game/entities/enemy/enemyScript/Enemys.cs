using Godot;
using System;
public partial class Enemys : CharacterBody2D
{
    public static readonly StringName EnemyGroup = "enemy";

    // States
    public enum State
    {
        Idle,
        Chase,
        InRange,
        Dead,
    }

    // Stats
    [ExportGroup("Health")]
    [Export] public int MaxHp = 1;

    [ExportGroup("Movement")]
    [Export] public float MoveSpeed = 80f;

    [ExportGroup("Attack")]
    [Export] public float AttackRange = 125f;
    [Export] public float AttackCooldown = 1.2f;
    [Export] public int Damage = 1;

    private int _hp;

    public State CurrentState { get; private set; } = State.Idle;
    public int Hp => _hp;
    public int Facing { get; private set; } = 1;

    // Lifecycle
    public override void _Ready()
    {
        _hp = MaxHp;
        AddToGroup(EnemyGroup);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentState == State.Dead)
            return;

        // Target
        Mc player = Mc.Instance;
        if (player == null)
        {
            CurrentState = State.Idle;
            return;
        }

        Vector2 toPlayer = player.GlobalPosition - GlobalPosition;

        // Facing
        if (toPlayer.X != 0f)
            Facing = toPlayer.X > 0f ? 1 : -1;

        // Movement
        if (toPlayer.LengthSquared() > AttackRange * AttackRange)
        {
            CurrentState = State.Chase;
            Velocity = toPlayer.Normalized() * MoveSpeed;
            MoveAndSlide();
        }
        else
        {
            CurrentState = State.InRange;
            Velocity = Vector2.Zero;
        }
    }

    // Damage
    public void TakeDamage(int amount)
    {
        if (CurrentState == State.Dead)
            return;

        _hp -= amount;
        if (_hp <= 0)
            Die();
    }

    private void Die()
    {
        CurrentState = State.Dead;
        QueueFree();
    }
}