using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float smoothing = 6f;
    public Transform lookAtTarget;
    public Transform positionTarget;
    public Transform sideView;
    //public AdvancedOptions advancedOptions;
    public KeyCode switchViewKey = KeyCode.Space;

    bool m_ShowingSideView;

    private void FixedUpdate ()
    {
        UpdateCamera ();
    }

    private void Update ()
    {
        if (Input.GetKeyDown (switchViewKey))
            m_ShowingSideView = !m_ShowingSideView;
    }

    private void UpdateCamera ()
    {
        if (m_ShowingSideView)
        {
            transform.position = sideView.position;
            transform.rotation = sideView.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, positionTarget.position, Time.deltaTime * smoothing);
            transform.LookAt(lookAtTarget);
        }
    }
    public void SetCameraRig(CameraRigStruct rig)
    {
        if(rig!=null)
        {
            lookAtTarget = rig.camTarget.transform;
            positionTarget = rig.camPosition.transform;
            sideView = rig.camSidePosition.transform;
        }
    }
}
