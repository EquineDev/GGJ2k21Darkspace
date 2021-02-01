using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Room))]
public class RoomEvent : MonoBehaviour
{
    protected Room room;
    // Start is called before the first frame update
    void Awake()
    {
        room = GetComponent<Room>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void triggerEvent()
    {

    }
}
