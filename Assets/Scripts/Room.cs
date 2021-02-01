using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Room : MonoBehaviour
{
    public List<Door> doors;

	private List<Interactable> interactables = new List<Interactable>();

    public AudioClip roomAmbience;
	public AudioClip shortDescription;
	public AudioClip longDescription;
	public AudioClip doorOpenSfx;
	public AudioClip doorLockedSfx;

    public TextMeshProUGUI roomNameUi;
  
    public string roomName, ItemToUseInRoom;
    public string[] roomAliases;
	public bool hasBeenEntered {
		get;
		private set;
	}

    // Start is called before the first frame update
    void Start()
    {
        interactables = GetComponentsInChildren<Interactable>().ToList();
        roomNameUi = GetComponentInChildren<TextMeshProUGUI>();
        roomNameUi.text = roomName;
    }

	public List<Interactable> GetInteractables()
	{
		return interactables;
	}

	public bool Take(Interactable i)
	{
		if (interactables.Contains(i) && i.isTakable)
		{
			interactables.Remove(i);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void OnRoomEnter()
	{
		var roomEvents = GetComponents<RoomEvent>();
		foreach (var roomEvent in roomEvents)
		{
			roomEvent.triggerEvent();
		}
		GameManager.instance.pc.backgroundMusic.PlayMusicLooping(ref roomAmbience);
		hasBeenEntered = true;
	}

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        foreach (Door door in doors)
        {
            Gizmos.DrawRay(transform.position, door.room.transform.position - transform.position);
        }
    }
}
