using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeEvent : RoomEvent
{
    public AudioClip soundOnEntry;
    public AudioPlayback audioSource;

    public override void triggerEvent()
    {
        base.triggerEvent();
        audioSource.PlayAudioOnce(ref soundOnEntry, AudioType.SoundEffect);
    }
}
