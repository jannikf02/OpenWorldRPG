using Godot;
using System;

public partial class CharacterBody3D : Godot.CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	public Node3D currentVehicle = null;
	public Node3D nearbyVehicle = null;

	public Camera3D head;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		head = GetNode<Camera3D>("Head");
		//Lock the mouse to the screen.
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		//Handle Rotation with the mouse.
		if (@event is InputEventMouseMotion mouseMotion)
		{
			// Rotation around the Y-axis (yaw)
			RotateObjectLocal(Vector3.Up, Mathf.DegToRad(-mouseMotion.Relative.X * 0.1f));
			if(head.Current){
				// Rotation Heaed only around the X-axis (pitch)
				float pitch = Mathf.Clamp(head.RotationDegrees.X + -mouseMotion.Relative.Y * 0.1f, -80, 80);
				head.RotationDegrees = new Vector3(pitch, 0, 0);
			}
			// else{
			// 	float pitch = Mathf.Clamp(anchor.RotationDegrees.X + -mouseMotion.Relative.Y * 0.1f, -15, 45);
			// 	anchor.RotationDegrees = new Vector3(pitch, 0, 0);
			// }
		}
	}
}
