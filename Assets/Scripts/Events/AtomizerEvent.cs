using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomizerEvent : Event
{
	public override void triggerEvent()
	{
		base.triggerEvent();
		
		GameManager.instance.pc.audioPlayback.PlayAudioOnce(ref sound, AudioType.SoundEffect);
		GameManager.instance.RestartDelay(sound?.length ?? 5f);
	}
}
