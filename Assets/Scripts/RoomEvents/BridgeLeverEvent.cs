using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BridgeLeverPosition {
    LEFT=1,
    MIDDLE=2,
    RIGHT=4
}

public class BridgeLeverEvent : RoomEvent
{
    public AudioClip leverSound;
    public AudioClip successSound;
    public static byte leverState;
    const byte leverCorrect = 5;
    public AudioPlayback audioSource;
    public BridgeLeverPosition bridgeLeverPosition;

    public bool leverStatus {
        get {
            return (leverState & (byte) bridgeLeverPosition) > 0;
        }
    }

    public override void triggerEvent()
    {
        leverState ^= (byte) bridgeLeverPosition;
        audioSource.EnqueueQueue(ref leverSound, AudioType.SoundEffect);

        base.triggerEvent();
        if (leverState == leverCorrect)
        {
            audioSource.EnqueueQueue(ref successSound, AudioType.SoundEffect);
            
        }
    }
}
