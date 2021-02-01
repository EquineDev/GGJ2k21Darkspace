using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioPlayback))]
[RequireComponent(typeof(AudioRoomLooping))]
public class PlayerCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        audioPlayback = GetComponent<AudioPlayback>();
        backgroundMusic = GetComponent<AudioRoomLooping>();
    }

    void Start() {
        roomOccupied.OnRoomEnter();
    }

    public Room roomOccupied;

	public List<Interactable> inventory = new List<Interactable>();

    public AudioClip walkSound;
    public AudioClip bumpIntoWallSound;
    public AudioClip doorDefaultSound;
    public AudioPlayback audioPlayback;
    public AudioRoomLooping backgroundMusic;

    // Update is called once per frame
    void Update()
    {
        transform.position = roomOccupied.transform.position - Vector3.forward;
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        // PublishInput.EmitPublisher += giveMoveInstruction;
    }

    public void giveMoveInstruction(string instruction)
    {
        Room resultRoom = null;
        resultRoom = getRoomByName(instruction) ?? getRoomByDirection(instruction);

        if (resultRoom == null)
        {
            audioPlayback.PlayAudioOnce(ref bumpIntoWallSound, AudioType.SoundEffect);
        }
        else if (resultRoom == roomOccupied)
        {
            // Locked door scenario
        }
        else {
            moveToRoom(resultRoom);
        }
    }

    public void moveToRoom(Room roomToMoveTo)
    {
        if (walkSound != null) 
        {
            audioPlayback.PlayAudioOnce(ref walkSound, AudioType.SoundEffect);
        }
        backgroundMusic.StopFade();
        audioPlayback.StopAudio(AudioType.VoiceOver);
        StartCoroutine(walkingTo(roomToMoveTo));
    }

    public IEnumerator walkingTo(Room roomToMoveTo)
    {
        yield return new WaitForSeconds(3);
        audioPlayback.PlayAudioOnce(ref roomOccupied.doorOpenSfx, AudioType.SoundEffect);
        roomOccupied = roomToMoveTo;
        transform.position = roomOccupied.transform.position;
        roomOccupied.OnRoomEnter();
    }

    public Room getRoomByName(string roomName) {
        Room resultRoom = null;
        foreach(Door door in roomOccupied.doors)
        {
            if (door.room.roomName == roomName)
            {
                if (isDoorLocked(door))
                {
                    audioPlayback.PlayAudioOnce(ref roomOccupied.doorLockedSfx, AudioType.SoundEffect);
                    if (door.lockReason != null)
                    {
                        audioPlayback.PlayAudioOnce(ref door.lockReason, AudioType.VoiceOver);
                    }
                    return roomOccupied;
                }
                else {
                    resultRoom = door.room;
                }
            }
        }
        return resultRoom;
    }

    public Room getRoomByDirection(string direction) {
        
        Room resultRoom = null;
        foreach(Door door in roomOccupied.doors)
        {
            if (door.direction == direction)
            {
                if (isDoorLocked(door))
                {
                    audioPlayback.PlayAudioOnce(ref roomOccupied.doorLockedSfx, AudioType.SoundEffect);
                    if (door.lockReason != null)
                    {
                        audioPlayback.PlayAudioOnce(ref door.lockReason, AudioType.VoiceOver);
                    }
                    return roomOccupied;
                }
                else
                {
                    resultRoom = door.room;
                }
            }
        }
        return resultRoom;
    }

    public bool isDoorLocked(Door door)
    {
        if (GameManager.instance.states.TryGetValue(door.lockCondition, out string lockState))
        {
            return lockState == "locked";
        }
        return false;
    }
}
