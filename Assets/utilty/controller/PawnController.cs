using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PawnController : ControllerBase
{
	public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
	public Vector3 Drag;

	private CharacterController _controller; //https://github.com/valgoun/CharacterController
	private Vector3 _velocity;
    private bool _isGrounded = true;
	private Transform _groundChecker;

    private bool _nearVehicle;
    private ControllerBase _vehicleControllerBase;

    void Start()
    {
    	base.CacheCameraRig();
        _controller = GetComponent<CharacterController>();
        _groundChecker = transform.GetChild(0);
    }

    public override void Control(InputManager im)
    {
        //Check if we are trying to enter a vehicle
        if(_nearVehicle && im.use)
        {
            //_vehicleControllerBase.Posess(im);
            Exorcise();
            im.Posess(_vehicleControllerBase);
            return;
        }

        /////////////////
    	_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

    	Vector3 move = new Vector3(im.steer, 0, im.throttle);
		_controller.Move(move * Time.deltaTime * Speed);
		//if (move != Vector3.zero)
		//	transform.forward = move;
        
		if (im.jump && _isGrounded)
        {
            Debug.Log(im.jump);
            _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        if (im.dash)
        {
            //Debug.Log("Dash");
            _velocity += Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
        }


        _velocity.y += Gravity * Time.deltaTime;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

		_controller.Move(_velocity * Time.deltaTime);
    }

    //for when we enter a trigger
    void OnTriggerEnter(Collider other) {
        //if (other.tag == "Dock") {
            _nearVehicle = true;
            _vehicleControllerBase = other.gameObject.GetComponent<ControllerBase>();
        //}
    }
     
    void OnTriggerExit(Collider other) {
        //if (other.tag == "Dock") {
            _nearVehicle = false;
        //}
    }
}
