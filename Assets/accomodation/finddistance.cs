using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class finddistance : MonoBehaviour
{
	public Camera cam;
    public Transform debug;
    public Transform debugSection;     
    private Vector3 focus_position;
    private Vector3 last_focus_position;

    public enum _findtype
    {
        angle,
        basic,
        ray,
        angleAndRay,
        angleAndBasic
    }

    [System.Serializable]
    public struct _sections
    {
        public GameObject section;
        public _findtype findtype;
        public GameObject[] children;   
    }

    
    public _sections[] sections;
    private int _inSection = -1;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            //lopp the sections, to see if we hit a new one
            int i=0;
            Vector3 hitPosition = new Vector3();
            foreach(_sections section in sections)
            {
                if (hit.transform.gameObject == section.section)
                {
                    _inSection = i;
                    hitPosition = hit.point;
                    if(debugSection!=null)
                    {
                        debugSection.position = hitPosition;
                    }
                }
                i++;
            }

        }
        ///we are in a section
        if(_inSection>=0)
        {
            //lets look at the sections children to focus on
            switch(sections[_inSection].findtype)
            {
                case _findtype.angle:
                    SetFocusPosition( CheckAngle(ray,sections[_inSection].children));
                    break;
                case _findtype.basic:
                    SetFocusPosition(sections[_inSection].children[0].transform.position);
                    break;
                case _findtype.ray:
                    SetFocusPosition(CheckRaycast(hit.point+ray.direction*0.01f,ray.direction));
                    break;
                case _findtype.angleAndRay:
                    //first we see if we are hitting something
                    Vector3 rayhit = CheckRaycast(hit.point+ray.direction*0.01f,ray.direction);
                    Vector3 angleTarget = CheckAngle(ray,sections[_inSection].children);
                    SetFocusPosition(Vector3.Lerp(angleTarget,rayhit,Mathf.Ceil(rayhit.magnitude-0.001f)));
                    break;
                case _findtype.angleAndBasic:
                    break;
            }
        }
        
    }
    private void SetFocusPosition(Vector3 pos)
    {
        if(pos.magnitude>0f)
        {
            focus_position = Vector3.Lerp(last_focus_position,pos,0.45f);
            last_focus_position = focus_position;

            if(debug!=null)
            {
                debug.position = focus_position;
            }
            ///HERE IS WHERE I CAN SET THE ACCOMODATION
            //////////////--------///////////////
            print(Vector3.Distance(cam.transform.position,pos));
            ///////////////////////////////
        }
    }
    private Vector3 CheckRaycast(Vector3 origin, Vector3 direction)
    {
        Ray ray = new Ray(origin,direction);//cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        Vector3 hitPosition = new Vector3();
        if (Physics.Raycast(ray, out hit)){
            hitPosition = hit.point;
        }
        return hitPosition;
    }
    private Vector3 CheckAngle(Ray ray, GameObject[] objects)
    {
        float dot = 0f;
        Vector3 inView = new Vector3();
        foreach(GameObject o in objects)
        {
            Vector3 oDir = o.transform.position-ray.origin;
            float oDot = Vector3.Dot(ray.direction,oDir.normalized);
            if(oDot>dot)
            {
                dot=oDot;
                inView = o.transform.position;
            }
        }
        return inView;
    }
}
