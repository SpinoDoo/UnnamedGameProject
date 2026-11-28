using Godot;

public partial class Mc : CharacterBody2D
{
    // Input
    private static readonly StringName MoveLeft = "move_left";
    private static readonly StringName MoveRight = "move_right";
    private static readonly StringName MoveUp = "move_up";
    private static readonly StringName MoveDown = "move_down";
    private static readonly StringName BlinkInput = "rolling";

    public static readonly StringName PlayerGroup = "player";

    public static Mc Instance { get; private set; }

    // Movement
    [ExportGroup("Movement")]
    [Export] public float MaxSpeed = 300f;
    [Export] public float Acceleration = 1500f;
    [Export] public float Friction = 1200f;

    // Blink
    [ExportGroup("Blink")]
    [Export] public float BlinkDistance = 180f;
    [Export] public float BlinkCooldown = 1f;
    [Export] public bool BlinkStopsAtObstacles = true;

    private float _blinkCooldownLeft;
    private Vector2 _facingDirection = Vector2.Right;

    public bool CanBlink => _blinkCooldownLeft <= 0f;
    public Vector2 FacingDirection => _facingDirection;

    // Lifecycle
    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void _Ready()
    {
        AddToGroup(PlayerGroup);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // Input
        Vector2 inputDirection = Input.GetVector(MoveLeft, MoveRight, MoveUp, MoveDown);
        if (inputDirection != Vector2.Zero)
            _facingDirection = inputDirection.Normalized();

        // Blink
        if (_blinkCooldownLeft > 0f)
            _blinkCooldownLeft -= dt;

        if (CanBlink && Input.IsActionJustPressed(BlinkInput))
            Blink();

        // Movement
        Vector2 targetVelocity = inputDirection * MaxSpeed;
        float maxChange = (inputDirection != Vector2.Zero ? Acceleration : Friction) * dt;
        Velocity = Velocity.MoveToward(targetVelocity, maxChange);

        MoveAndSlide();
    }

    // Blink
    private void Blink()
    {
        Vector2 offset = _facingDirection * BlinkDistance;

        if (BlinkStopsAtObstacles)
        {
            KinematicCollision2D hit = MoveAndCollide(offset, testOnly: true);
            if (hit != null)
                offset = hit.GetTravel();
        }

        GlobalPosition += offset;
        _blinkCooldownLeft = BlinkCooldown;
    }
}