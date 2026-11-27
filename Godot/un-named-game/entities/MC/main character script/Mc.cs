//using Godot;
//using System;

//public partial class Mc : CharacterBody2D
//{
//    public const float Speed = 300.0f;
//    const float Acceleration = 1500f;
//    const float Friction = 1200f;

//    private float _rollCdTimer = 0.0f;
//    private bool _isRolling = false;
//    private float RollSpeed = 50;
//    private float RollDuration = 3;
//    private Vector2 _rollDirection;
//    public const float RollCooldown = 1.0f;
//    bool IsRollingAvailable = true;
//    public Vector2 EndPosition;

//    [Export] public int MaxHp = 5;
//    private int _hp;

//    [Export] public int AttackDamage = 1;
//    [Export] public float AttackRange = 150f;
//    [Export] public float AttackCooldown = 0.25f;
//    [Export] public float AttackDuration = 0.12f;

//    private float _attackTimer = 0f;
//    private float _cooldown;
//    private bool _attacking = false;

//    public override void _Ready()
//    {
//        _hp = MaxHp;
//        AddToGroup("player");
//    }

//    public override void _PhysicsProcess(double delta)
//    {
//        float dt = (float)delta;
//        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

//        if (_cooldown > 0f)
//            _cooldown -= dt;

//        if (_attackTimer > 0f)
//        {
//            _attackTimer -= dt;
//            if (_attackTimer <= 0f)
//                _attacking = false;
//        }

//        if (_rollCdTimer > 0f)
//        {
//            _rollCdTimer -= dt;
//            IsRollingAvailable = false;
//        }
//        else
//        {
//            IsRollingAvailable = true;
//        }

//        if (Input.IsActionJustPressed("rolling") && !_isRolling && IsRollingAvailable && !_attacking)
//        {
//            _isRolling = true;
//            _rollDirection = direction != Vector2.Zero ? direction.Normalized() : Vector2.Right;
//            EndPosition = GlobalPosition + (_rollDirection * RollSpeed * RollDuration);
//        }

//        if (Input.IsActionJustPressed("Attack") && _cooldown <= 0f && !_attacking && !_isRolling)
//        {
//            _attacking = true;
//            _attackTimer = AttackDuration;
//            _cooldown = AttackCooldown;
//            StartAttack();
//        }

//        if (_attacking)
//        {
//            Velocity = Vector2.Zero;
//        }
//        else if (_isRolling && IsRollingAvailable)
//        {
//            GlobalPosition = EndPosition;
//            _isRolling = false;
//            _rollCdTimer = RollCooldown;
//        }
//        else if (direction != Vector2.Zero)
//        {
//            Velocity = Velocity.MoveToward(direction * Speed, Acceleration * dt);
//        }
//        else
//        {
//            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * dt);
//        }

//        MoveAndSlide();
//    }

//    public void TakeDamage(int amount)
//    {
//        if (_hp <= 0) return;

//        _hp -= amount;
//        GD.Print($"Hp : {_hp}");

//        if (_hp <= 0)
//        {
//            GD.Print("A player meghalt");
//        }
//    }

//    private void StartAttack()
//    {
//        float rangeSq = AttackRange * AttackRange;

//        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
//        {
//            if (node is not Enemys enemy || !IsInstanceValid(enemy)) continue;

//            Vector2 d = enemy.GlobalPosition - GlobalPosition;
//            if (d.LengthSquared() <= rangeSq)
//            {
//                enemy.TakeDamage(AttackDamage);
//            }
//        }
//    }
//}


using Godot;
using System;

public partial class Mc : CharacterBody2D
{

    public const float Speed = 300.0f;
    const float Acceleration = 1500f;
    const float Friction = 1200f;
    private float _rollCdTimer = 0.0f;
    private bool _isRolling = false;
    private Vector2 _rollDirection;
    // Duration of the rolling effect in seconds
    public const float RollSpeed = 450.0f;
    public const float RollDuration = 0.4f;
    public const float RollCooldown = 1.0f; // Cooldown time between rolls in seconds
    bool IsRollingAvailable = true;
    public Vector2 EndPosition;

    // --- Player HP / TakeDamage ---
    [Export] public int MaxHp = 5;
    private int _hp;

    // --- Attack + _Draw() részhez tartozó mezők ---
    [Export] public Area2D AttackArea;
    [Export] public float AttackRange = 140.0f;
    [Export] public float AttackArcDegrees = 180.0f; // 180 = félkörív az egér felé
    [Export] public int AttackDamage = 1;
    [Export] public float AttackCooldown = 0.4f;
    [Export] public float SlashVisualDuration = 0.14f;
    private float _attackCdTimer = 0.0f;
    private float _slashVisualTimer = 0.0f;




    public override void _Ready()
    {
        _hp = MaxHp;
        AddToGroup("player");

        AttackArea ??= GetNodeOrNull<Area2D>("AttackArea");
        SyncAttackAreaRadius();
    }



    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        float dt = (float)delta;



        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (_isRolling && IsRollingAvailable)
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

        if (_rollCdTimer > 0)
        {
            _rollCdTimer -= dt;
            IsRollingAvailable = false;
        }
        else
        {
            IsRollingAvailable = true;
        }

        // --- Attack cooldown és vizuális időzítő ---
        if (_attackCdTimer > 0)
        {
            _attackCdTimer -= dt;
        }

        if (_slashVisualTimer > 0)
        {
            _slashVisualTimer = Mathf.Max(0.0f, _slashVisualTimer - dt);
        }

        if (Input.IsActionJustPressed("Attack") && _attackCdTimer <= 0)
        {
            _attackCdTimer = AttackCooldown;
            _slashVisualTimer = SlashVisualDuration;
            Attack();
        }

        QueueRedraw();

        velocity = Velocity;
        MoveAndSlide();
    }

    public void TakeDamage(int amount)
    {
        if (_hp <= 0) return;

        _hp -= amount;
        GD.Print($"Hp : {_hp}");

        if (_hp <= 0)
        {
            GD.Print("A player meghalt");
        }
    }

    private void SyncAttackAreaRadius()
    {
        if (AttackArea == null)
            return;

        foreach (Node child in AttackArea.GetChildren())
        {
            if (child is CollisionShape2D shapeNode && shapeNode.Shape is CircleShape2D circle)
            {
                circle.Radius = AttackRange;
            }
        }
    }

    private void Attack()
    {
        if (AttackArea == null)
            return;

        Vector2 attackDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        if (attackDir == Vector2.Zero)
            attackDir = Vector2.Right;

        float rangeSq = AttackRange * AttackRange;
        float minDot = Mathf.Cos(Mathf.DegToRad(AttackArcDegrees * 0.5f));

        foreach (Node2D body in AttackArea.GetOverlappingBodies())
        {
            if (body is not Enemys enemy)
                continue;

            Vector2 toEnemy = enemy.GlobalPosition - GlobalPosition;
            float distSq = toEnemy.LengthSquared();

            if (distSq > rangeSq)
                continue;

            Vector2 dirToEnemy = distSq > 0.0001f ? toEnemy.Normalized() : attackDir;
            if (attackDir.Dot(dirToEnemy) >= minDot)
            {
                enemy.TakeDamage(AttackDamage);
            }
        }
    }

    public override void _Draw()
    {
        Vector2 attackDir = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        if (attackDir == Vector2.Zero)
            attackDir = Vector2.Right;

        float centerAngle = attackDir.Angle();
        float halfArc = Mathf.DegToRad(AttackArcDegrees * 0.5f);
        float startAngle = centerAngle - halfArc;
        float endAngle = centerAngle + halfArc;

        bool isActiveSlash = _slashVisualTimer > 0.0f;
        Color arcColor = isActiveSlash
            ? new Color(1.0f, 0.85f, 0.2f, 0.95f)
            : new Color(1.0f, 1.0f, 1.0f, 0.28f);
        float lineWidth = isActiveSlash ? 4.0f : 2.0f;

        DrawArc(Vector2.Zero, AttackRange, startAngle, endAngle, 28, arcColor, lineWidth);

        Vector2 leftEdge = Vector2.FromAngle(startAngle) * AttackRange;
        Vector2 rightEdge = Vector2.FromAngle(endAngle) * AttackRange;
        DrawLine(Vector2.Zero, leftEdge, arcColor, lineWidth);
        DrawLine(Vector2.Zero, rightEdge, arcColor, lineWidth);
    }
}
