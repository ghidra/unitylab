using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finddistance : MonoBehaviour
{
	public Camera cam;
    public Transform debug;    
    private Vector3 focus_position;
    private Vector3 last_focus_position;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            //print("I'm looking at " + hit.transform.name);
            focus_position = Vector3.Lerp(last_focus_position,hit.point,0.6f);
            last_focus_position = focus_position;

            debug.position = focus_position;
        }
        
    }
}
