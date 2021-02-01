using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door
{
    [SerializeField]
    public Room room;
    public string direction;
    public AudioClip lockReason;

    public string lockCondition;

}
