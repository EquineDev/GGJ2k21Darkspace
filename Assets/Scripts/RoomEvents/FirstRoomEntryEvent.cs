using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomEntryEvent : RoomEvent
{
    public AudioClip soundOnEntry;
    public override void triggerEvent()
    {
        base.triggerEvent();
        if (room.hasBeenEntered)
        {
            return;
        }
        GameManager.instance.audioSource.PlayAudioOnce(ref soundOnEntry, AudioType.VoiceOver);
    }
}
