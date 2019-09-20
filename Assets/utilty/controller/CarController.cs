using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : ControllerBase
{
	public List<WheelCollider> throttleWheels;
	public List<WheelCollider> steeringWheels;
    public Transform CoG;

    [Tooltip("Maximum steering angle of the wheels")]
	public float maxTurn = 30f;
    [Tooltip("Maximum torque applied to the driving wheels")]
    public float maxTorque = 300f;
    [Tooltip("Maximum brake torque applied to the driving wheels")]
    public float brakeTorque = 30000f;

    [Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
    public float criticalSpeed = 5f;
    [Tooltip("Simulation sub-steps when the speed is above critical.")]
    public int stepsBelow = 5;
    [Tooltip("Simulation sub-steps when the speed is below critical.")]
    public int stepsAbove = 1;

    // Update is called once per frame
    void Start()
    {
    	//im=GetComponent<InputManager>();
        base.CacheCameraRig();
    }
    //void FixedUpdate()
    //{
    public override void Control(InputManager im)
    {
        if(im!=null)
        {
            if(im.use)
            {
                Exorcise();
                im.Reposess(GetExitPosition());
                return;
            }
            throttleWheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);
            foreach(WheelCollider wheel in throttleWheels)
            {
                if(im.handBrake>0f)
                {
                    wheel.brakeTorque = im.handBrake;
                }
                else
                {
                    wheel.motorTorque = maxTorque * im.throttle;
                }

                //transform the wheels
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose (out p, out q);

                // Assume that the only child of the wheelcollider is the wheel shape.
                Transform shapeTransform = wheel.transform.GetChild (0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
            foreach(WheelCollider wheel in steeringWheels)
            {
            	wheel.steerAngle = maxTurn * im.steer;

                //transform the wheels
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose (out p, out q);

                // Assume that the only child of the wheelcollider is the wheel shape.
                Transform shapeTransform = wheel.transform.GetChild (0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }
        }

    }
}
