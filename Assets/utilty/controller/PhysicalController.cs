using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalController : ControllerBase
{
	public float Speed = 5f;
    public float DashDistance = 5f;
	public Vector3 Drag;

	private Rigidbody _body;
	private Vector3 _velocity;

    void Start()
    {
    	//base.CacheCameraRig();
        _body = GetComponent<Rigidbody>();
    }

    public override void Control(InputManager im)
    {

    	Vector3 move = new Vector3(im.steer, 0, im.throttle);
		_body.AddForce(move);

        if (im.dash)
        {
            //Debug.Log("Dash");
            _velocity += Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * Drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * Drag.z + 1)) / -Time.deltaTime)));
        }


        //_velocity.y += Gravity * Time.deltaTime;

        _velocity.x /= 1 + Drag.x * Time.deltaTime;
        _velocity.y /= 1 + Drag.y * Time.deltaTime;
        _velocity.z /= 1 + Drag.z * Time.deltaTime;

		_body.AddForce(_velocity * Time.deltaTime);
    }
}
