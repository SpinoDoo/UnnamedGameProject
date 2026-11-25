using Godot;
using System;

public partial class Mc : CharacterBody2D
{
    
	public const float Speed = 300.0f;
    const float Acceleration = 1500f;
    const float Friction = 1200f;
    private float _rollCdTimer = 0.0f;
    private bool _isRolling = false;
    private float RollSpeed = 50;
    private float RollDuration = 3;
    private Vector2 _rollDirection;
    public const float RollCooldown = 1.0f; // Cooldown time between rolls in seconds
    bool IsRollingAvailable = true;
    public Vector2 EndPosition;

    [Export] public int MaxHp = 5;
    private int _hp;


    [Export] public int AttackDamage = 1;
    [Export] public float AttackRange = 150f;
    [Export] public float AttackCooldown = 2f;
    [Export] public float AttackDuration = 1f;

    private float _attackTimer = 0f;
    private float _cooldown;
    private float _attackDmg;
    private bool _attacking = false;
    


    public override void _Ready()
    {
        _hp = MaxHp;
        _attackDmg = AttackDamage;
        AddToGroup("player");
    }



    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
        float dt = (float)delta;
        


        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (_cooldown >= 0f)
            _cooldown -= dt;

        if (_attackTimer >= 0f)
        {
            _attackTimer -= dt;
            if (_attackTimer <= 0f)
                _attacking = false;
        }

        if (Input.IsActionJustPressed("Attack")
            && _cooldown <= 0
            && _isRolling == false)
        {
            _attacking = true;
            _attackTimer = AttackDuration;
            _cooldown = AttackCooldown;
            StartAttack();
        }


        if (_attacking)
        {
            Velocity = Vector2.Zero;
        }
        else if (_isRolling && IsRollingAvailable)
        {
            GlobalPosition = EndPosition;
            _isRolling = false;
            _rollCdTimer = RollCooldown;
        }
        else if (direction != Vector2.Zero)
		{
            
            Velocity = Velocity.MoveToward(direction * Speed, Acceleration * dt);
        }
		else
		{
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * dt);
        }

        if (Input.IsActionJustPressed("rolling") && !_isRolling && IsRollingAvailable)
        {
            _isRolling = true;
            
            _rollDirection = direction != Vector2.Zero ? direction.Normalized() : Vector2.Right;
            EndPosition = GlobalPosition + (_rollDirection * RollSpeed * RollDuration);
        }

        if(_rollCdTimer > 0)
        {
            _rollCdTimer -= dt;
            IsRollingAvailable = false;
        }
        else
        {
            IsRollingAvailable = true;
        }




        velocity = Velocity;
        MoveAndSlide();
	}

    public void TakeDamage(int amount)
    {
        _hp -= amount;
        GD.Print($"Hp : {_hp}");

        //Game Over
        if (_hp <= 0)
        {
            GD.Print("A player meghalt");
        }
    }


    private void StartAttack()
    {
        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
        {
            if (node is not Node2D enemy || !IsInstanceValid(enemy)) continue;

            Vector2 d = enemy.GlobalPosition - GlobalPosition;

            float distSq = d.LengthSquared();
            float rangeSq = AttackRange * AttackRange;
            if (distSq < rangeSq && enemy.HasMethod("TakeDamage")) 
            {
                enemy.Call("TakeDamage", AttackDamage);
            }
        }
    }


}
