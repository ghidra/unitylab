using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp_swimming : MonoBehaviour
{
	private Vector3 _StartPosition;
	[Range(0,1)]
	public float speed=1.0f;
	[Range(0,10)]
	public float distance=1.0f; 
    // Start is called before the first frame update
    void Start()
    {
        _StartPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    	float nx = Mathf.PerlinNoise(_StartPosition.x+Time.time*speed,_StartPosition.y+Time.time*speed);
        float ny = Mathf.PerlinNoise(_StartPosition.x+Time.time*speed-1232.331f,_StartPosition.y+Time.time*speed+45.909f);
        float nz = Mathf.PerlinNoise(_StartPosition.x+Time.time*speed+137.137f,_StartPosition.y+Time.time*speed+8845.099f);

        Vector3 np = new Vector3(nx,ny,nz);
        transform.position = np * distance + _StartPosition;
    }
}
