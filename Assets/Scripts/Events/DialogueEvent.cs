using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent : Event
{
	private void Start()
	{
		GameManager.instance.AddEvent(this);
	}

	public override void triggerEvent()
	{
		base.triggerEvent();
		
		GameManager.instance.pc.audioPlayback.PlayAudioOnce(ref sound, AudioType.VoiceOver);
	}
}
