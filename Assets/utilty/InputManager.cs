using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public float throttle;
	public float steer;
	public float handBrake;
    public bool jump;
    public bool dash;
    public bool use;
    // Update is called once per frame
    public ControllerBase control;
    public CameraManager cameramanager;

    private ControllerBase main_control;//this keeps our main controller, that we can go back to
    void Start()
    {
        main_control = control;
        this.Posess(control);
        //control.Posess(this);
    }
    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        handBrake = Input.GetKey(KeyCode.X) ? 1f : 0f;

        jump = Input.GetButtonDown("Jump");
        //jump = Input.GetKeyDown(KeyCode.Space);
        dash = Input.GetKeyDown(KeyCode.LeftShift);
        use = Input.GetKeyDown(KeyCode.E);

        //control.Control(this);
    }
    void FixedUpdate()
    {
        control.Control(this);
    }
    /*public virtual void SetCameraRig(CameraRigStruct rig)
    {
        //
        //Debug.Log("trying to set the camea rig");
        
        cameramanager.SetCameraRig(rig);
    }*/
    public void Posess(ControllerBase controller)
    {
        control = controller;
        control.Posess(this);
        cameramanager.SetCameraRig(control.getCameraRigStruct());
    }
    public void Reposess(Vector3 position)
    {
        //set from ext rig position
        main_control.gameObject.transform.position = position;
        //then do normal posession
        Posess(main_control);
    }
}
