using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBase : MonoBehaviour
{
	public InputManager im;
    protected bool posessed;
	private CameraRigStruct camRigStruct;
    private List<GameObject> exitPoints;

	public virtual void CacheCameraRig()
	{
        //we are hoping there is a camera rig in here
		GameObject camRig = gameObject.transform.Find("CameraRig").gameObject;
		if(camRig!=null)
		{
			camRigStruct = new CameraRigStruct();
			camRigStruct.camTarget = camRig.transform.Find("camTarget").gameObject;
			camRigStruct.camPosition = camRig.transform.Find("camPosition").gameObject;
			camRigStruct.camSidePosition = camRig.transform.Find("camSidePosition").gameObject;
		}

        //lets now test to see if there are exits. for dismounting
        GameObject exitRig = gameObject.transform.Find("Exits").gameObject;
        if(exitRig!=null)
        {
            exitPoints = new List<GameObject>();
            foreach(Transform child in exitRig.transform)
            {
                exitPoints.Add(child.gameObject);
            }
            Debug.Log(exitPoints.Count);
        }
	}
    
    public void Posess(InputManager inputmanager)
    {
        im = inputmanager;
        //see if we have a camera rig to pass along
        /*if(camRigStruct!=null)
        {
        	im.SetCameraRig(camRigStruct);
        }*/

        posessed = true;
    }

    // Update is called once per frame
    public void Exorcise()
    {
        im = null;
        posessed = false;
    }

    public CameraRigStruct getCameraRigStruct()
    {
        return camRigStruct;
    }
    public Vector3 GetExitPosition()
    {
        return exitPoints[0].transform.position;
    }

    public virtual void Control(InputManager im){}
}
