using Godot;
using System;

public partial class Mc : CharacterBody2D
{
	public const float Speed = 300.0f;
    const float Acceleration = 1500f;
    const float Friction = 1200f;

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;




        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (direction != Vector2.Zero)
		{
            GD.Print(direction, " ", Velocity);
            Velocity = Velocity.MoveToward(direction * Speed, Acceleration * (float)delta);
        }
		else
		{
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * (float)delta);
        }

		MoveAndSlide();
	}
}
