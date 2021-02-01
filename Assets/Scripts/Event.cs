using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
	public AudioClip sound;

	public string eventName = "";

	// Start is called before the first frame update
	protected virtual void Start()
    {
		GameManager.instance.AddEvent(this);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void triggerEvent()
    {
		if (sound)
		{
			GameManager.instance.audioSource.PlayAudioOnce(ref sound, AudioType.SoundEffect);
		}
		
	}
}
