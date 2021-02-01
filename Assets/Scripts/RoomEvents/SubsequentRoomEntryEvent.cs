using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubsequentRoomEntryEvent : RoomEvent
{
    public AudioClip soundOnEntry;

    public void Awake()
    {
        room = GetComponent<Room>();
    }

    public override void triggerEvent()
    {
        base.triggerEvent();
        if (!room.hasBeenEntered)
        {
            return;
        }
        GameManager.instance.audioSource.PlayAudioOnce(ref soundOnEntry, AudioType.VoiceOver);
    }
}
