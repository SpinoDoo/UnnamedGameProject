using Godot;
using System;

public partial class Mc : CharacterBody2D
{
	public const float Speed = 300.0f;
    const float Acceleration = 1500f;
    const float Friction = 1200f;
    private float _rollTimer = 0.0f;
    private bool _isRolling = false;
    private Vector2 _rollDirection;
     // Duration of the rolling effect in seconds
    public const float RollSpeed = 450.0f;
    public const float RollDuration = 0.4f;
    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
        float dt = (float)delta;



        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
            
            Velocity = Velocity.MoveToward(direction * Speed, Acceleration * dt);
        }
		else
		{
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * dt);
        }

        if (Input.IsActionJustPressed("rolling") && !_isRolling)
        {
            _isRolling = true;
            _rollTimer = RollDuration;
            _rollDirection = direction != Vector2.Zero ? direction.Normalized() : Vector2.Right;
        }

        if (_isRolling)
        {
            _rollTimer -= dt;
            if (_rollTimer <= 0.0f)
            {
                _isRolling = false;
            }
            else
            {
                // Apply rolling movement
                Velocity = _rollDirection * RollSpeed;
            }
        }

        velocity = Velocity;
        MoveAndSlide();
	}
}
