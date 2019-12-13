using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
	public GameObject _simpleBullet;
	[Range(16,256)]
	public int _simpleBullets=16;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(_simpleBullet, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
