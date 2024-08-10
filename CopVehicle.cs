using Godot;
using System;

public partial class CopVehicle : VehicleBody3D
{
	private CharacterBody3D currentDriver = null;
	private CharacterBody3D nearbyPlayer = null;
	private Camera3D playerCam = null;

	private Node3D anchor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anchor = GetNode<Node3D>("CamAnchor");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Print current Position
	}

	public override void _PhysicsProcess(double delta)
	{
		if(Input.IsActionJustReleased("veh_enter")){
			if(currentDriver == null && nearbyPlayer != null){
				currentDriver = nearbyPlayer;
				// playerCam = currentDriver.head.Current ? currentDriver.head : currentDriver.GetNode<PlayerCharacter>("CharacterBody3D").thirdPerson;
				playerCam = currentDriver.head;
				currentDriver.currentVehicle = this;
				currentDriver.Visible = false;
				anchor.GetNode<Camera3D>("DriverCam").MakeCurrent();
			}else if(currentDriver != null){
				currentDriver.currentVehicle = null;
				currentDriver.Visible = true;
				currentDriver = null;
				playerCam.MakeCurrent();
				// Make the player exit the vehicle slightly to the left of the current position of the vehicle
				Transform3D newTransform = currentDriver.GlobalTransform;
				newTransform.Origin = GlobalTransform.Origin;
				currentDriver.GlobalTransform = newTransform;
			}
		}
		if (currentDriver != null)
		{
			float currentSpeed = LinearVelocity.Length();
			float maxSpeed = 100.0f; // Define a maximum speed
			float maxSteeringFactor = 1.0f; // Define a maximum steering factor

			// Calculate the steering factor based on the current speed
			float steeringFactor = Mathf.Lerp(0.4f, maxSteeringFactor, currentSpeed / maxSpeed);

			Steering = Input.GetAxis("char_right", "char_left") * steeringFactor;
			EngineForce = Input.GetAxis("char_back", "char_fwd") * 200f;
		}
	}

	private void _on_area_3d_body_entered(Node3D body)
	{
		if (body is CharacterBody3D charBody3D)
		{
			charBody3D.nearbyVehicle = this;
			nearbyPlayer = charBody3D;
		}
	}
	private void _on_area_3d_body_exited(Node3D body)
	{
		if (body is CharacterBody3D charBody3D)
		{
			charBody3D.nearbyVehicle = null;
			nearbyPlayer = null;
		}
	}

	public override void _Input(InputEvent @event)
	{
		//Handle Rotation with the mouse.
		if (@event is InputEventMouseMotion mouseMotion)
		{
			anchor.RotateObjectLocal(Vector3.Up, Mathf.Clamp(-mouseMotion.Relative.X * 0.01f, -1.0f, 1.0f));
		}
	}
}
