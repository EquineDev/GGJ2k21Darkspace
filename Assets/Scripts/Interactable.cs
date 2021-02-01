using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

	public string defaultInteraction;

	[System.Serializable]
	public struct Dict
	{
		public string key;
		public string value;
	}

	[SerializeField]
	public Dict[] verbsDict;

	[SerializeField]
	public Dict[] statesDict;

	[SerializeField]
	public Dict[] verbTargetsDict;

	//action the player commanded, name of the state to change
	public Dictionary<string, string> verbs = new Dictionary<string, string>();

	//name of state, new value for the state
	public Dictionary<string, string> states = new Dictionary<string, string>();

	//if a verb exists in the verbTargets list that means it can ONLY be done if the target is specified.
	//action player command, name of the second target that must be available to use with
	public Dictionary<string, string> verbTargets = new Dictionary<string, string>();

	public AudioClip interact;
	public AudioClip description;
	public new AudioClip name;

    private AudioPlayback audioSource;

	public string nameString;

	public bool isTakable = false;

	public bool isOneTimeUse = false;

	protected bool hasBeenUsed = false;

	private void Awake()
	{
        InitAwake();

    }

	private void Start()
	{
        InitStart();
    }

	public virtual void Interact(string action)
	{
		if(action == "")
		{

			action = defaultInteraction;
		}

		if (isOneTimeUse && hasBeenUsed)
		{
			//You have already used this and can't do it again audio clip.
			Debug.Log("You have already used this and can't do it again.");
		}
		else
		{

			if (verbs.ContainsKey(action) && !verbTargets.ContainsKey(action))
			{
				if (isOneTimeUse)
				{
					hasBeenUsed = true;
				}

				if (states[verbs[action]] == "event")
				{
					GameManager.instance.events[verbs[action]].triggerEvent();
					audioSource.PlayAudioOnce(ref interact, AudioType.SoundEffect);
				}
				else
				{
					GameManager.instance.states[verbs[action]] = states[verbs[action]];
					audioSource.PlayAudioOnce(ref interact, AudioType.SoundEffect);
				}
			}
			else
			{
				//You can't do that.
				Debug.Log("That requires something else to do.");
			}
		}
	}

	public virtual void Interact(string action, string target)
	{
		Debug.Log("do" + action + " on " + target);

		if (action == "")
		{
            
            action = defaultInteraction;
		}

		if (isOneTimeUse && hasBeenUsed)
		{
			//You have already used this and can't do it again audio clip.
			Debug.Log("You have already used this and can't do it again.");
		}
		else
		{

			if (verbs.ContainsKey(action) && verbTargets.ContainsKey(target) && verbTargets[action] == target)
			{
				if (isOneTimeUse)
				{
					hasBeenUsed = true;
				}

				GameManager.instance.states[verbs[action]] = states[verbs[action]];
				audioSource.PlayAudioOnce(ref interact, AudioType.SoundEffect);
			}
			else
			{
				//That won't work.
				Debug.Log("That won't work.");
			}


		}
	}

    protected virtual void InitAwake()
    {
        foreach (Dict d in verbsDict)
        {
            verbs.Add(d.key, d.value);
        }

        foreach (Dict d in statesDict)
        {
            states.Add(d.key, d.value);
        }

        foreach (Dict d in verbTargetsDict)
        {
            verbTargets.Add(d.key, d.value);
        }
    }
    protected virtual void InitStart()
    {
        GameManager.instance.AddNoun(nameString);
        audioSource = GameManager.instance.audioSource;
    }

   
}
