using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class camera_simple : MonoBehaviour {
 
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
 
	public float minimumX = -360F;
	public float maximumX = 360F;
 
	public float minimumY = -60F;
	public float maximumY = 60F;

	public float cameraSpeed = 2f;

	private bool mousedown = false;
	private bool wdown = false;
	private bool adown = false;
	private bool sdown = false;
	private bool ddown = false;
	private bool qdown = false;
	private bool edown = false;
 
	float rotationX = 0F;
	float rotationY = 0F;
 
	private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;	
 
	private List<float> rotArrayY = new List<float>();
	float rotAverageY = 0F;
 
	public float frameCounter = 20;
 
	Quaternion originalRotation;
 
	void Update ()
	{
		if(Input.GetMouseButtonDown(0))
		{
			mousedown=true;
		}
		if(Input.GetMouseButtonUp(0))
		{
			mousedown=false;
		}
		if(mousedown)
		{
			if (axes == RotationAxes.MouseXAndY)
			{			
				rotAverageY = 0f;
				rotAverageX = 0f;
	 
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
	 
				rotArrayY.Add(rotationY);
				rotArrayX.Add(rotationX);
	 
				if (rotArrayY.Count >= frameCounter) {
					rotArrayY.RemoveAt(0);
				}
				if (rotArrayX.Count >= frameCounter) {
					rotArrayX.RemoveAt(0);
				}
	 
				for(int j = 0; j < rotArrayY.Count; j++) {
					rotAverageY += rotArrayY[j];
				}
				for(int i = 0; i < rotArrayX.Count; i++) {
					rotAverageX += rotArrayX[i];
				}
	 
				rotAverageY /= rotArrayY.Count;
				rotAverageX /= rotArrayX.Count;
	 
				rotAverageY = ClampAngle (rotAverageY, minimumY, maximumY);
				rotAverageX = ClampAngle (rotAverageX, minimumX, maximumX);
	 
				Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
				Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
	 
				transform.localRotation = originalRotation * xQuaternion * yQuaternion;
			}
			else if (axes == RotationAxes.MouseX)
			{			
				rotAverageX = 0f;
	 
				rotationX += Input.GetAxis("Mouse X") * sensitivityX;
	 
				rotArrayX.Add(rotationX);
	 
				if (rotArrayX.Count >= frameCounter) {
					rotArrayX.RemoveAt(0);
				}
				for(int i = 0; i < rotArrayX.Count; i++) {
					rotAverageX += rotArrayX[i];
				}
				rotAverageX /= rotArrayX.Count;
	 
				rotAverageX = ClampAngle (rotAverageX, minimumX, maximumX);
	 
				Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
				transform.localRotation = originalRotation * xQuaternion;			
			}
			else
			{			
				rotAverageY = 0f;
	 
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
	 
				rotArrayY.Add(rotationY);
	 
				if (rotArrayY.Count >= frameCounter) {
					rotArrayY.RemoveAt(0);
				}
				for(int j = 0; j < rotArrayY.Count; j++) {
					rotAverageY += rotArrayY[j];
				}
				rotAverageY /= rotArrayY.Count;
	 
				rotAverageY = ClampAngle (rotAverageY, minimumY, maximumY);
	 
				Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
				transform.localRotation = originalRotation * yQuaternion;
			}
		}

		////keys
		if(Input.GetKeyDown("w")) wdown=true;
		if(Input.GetKeyDown("a")) adown=true;
		if(Input.GetKeyDown("s")) sdown=true;
		if(Input.GetKeyDown("d")) ddown=true;
		if(Input.GetKeyDown("q")) qdown=true;
		if(Input.GetKeyDown("e")) edown=true;

		if(Input.GetKeyUp("w")) wdown=false;
		if(Input.GetKeyUp("a")) adown=false;
		if(Input.GetKeyUp("s")) sdown=false;
		if(Input.GetKeyUp("d")) ddown=false;
		if(Input.GetKeyUp("q")) qdown=false;
		if(Input.GetKeyUp("e")) edown=false;

		if(wdown) transform.Translate(Vector3.forward*Time.deltaTime*cameraSpeed,Space.Self);
		if(sdown) transform.Translate(Vector3.back*Time.deltaTime*cameraSpeed,Space.Self);
		if(adown) transform.Translate(Vector3.left*Time.deltaTime*cameraSpeed,Space.Self);
		if(ddown) transform.Translate(Vector3.right*Time.deltaTime*cameraSpeed,Space.Self);
		if(qdown) transform.Translate(Vector3.up*Time.deltaTime*cameraSpeed,Space.Self);
		if(edown) transform.Translate(Vector3.down*Time.deltaTime*cameraSpeed,Space.Self);
	}
 
	void Start ()
	{		
        Rigidbody rb = GetComponent<Rigidbody>();	
		if (rb)
			rb.freezeRotation = true;
		originalRotation = transform.localRotation;
	}
 
	public static float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}			
		}
		return Mathf.Clamp (angle, min, max);
	}
}