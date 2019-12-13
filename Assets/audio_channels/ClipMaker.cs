using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClipMaker : MonoBehaviour
{
	public AudioClip clip;
    // Start is called before the first frame update
    private AudioClip _modifiedClip1;
    private AudioClip _modifiedClip2;

    private AudioSource _source;
    void Start()
    {
    	///make the audioclip
        _modifiedClip1 = AudioClipExtensions.CreateChanelSpecificClips(clip,1,2,"t1");
        _modifiedClip2 = AudioClipExtensions.CreateChanelSpecificClips(clip,1,3,"t2");

        _source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonInteract1()
    {
    	_source.clip = _modifiedClip1;
    	_source.Play();
    }
    public void ButtonInteract2()
    {
    	_source.clip = _modifiedClip2;
    	_source.Play();
    }
}
