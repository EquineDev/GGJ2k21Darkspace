using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
	public AudioClip pickUp;

	private int currentCode = 0;

	public AudioClip[] codeClips;
    public EndingDeath endingDeath;
	public Dictionary<string, string> states = new Dictionary<string, string> {
		{ "power", "off" },
		{ "nav", "compromised" },
		{ "captain", "sick" },
		{ "levers", "000" },
		{ "medbaycode", "locked" },
		{ "enginedoor", "locked" },
		{ "bridgecard", "locked" }
	};
	public Dictionary<string, Event> events = new Dictionary<string, Event>();
	public PlayerCharacter pc;
	public AudioPlayback audioSource;

	private List<string> properNouns = new List<string>();

	public static GameManager instance = null;
	private void Awake()
	{
		//Make sure this is the only Game Manager
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		//DontDestroyOnLoad(gameObject);

		GameObject existingGameManager = GameObject.Find("GameManager");
		if (existingGameManager != gameObject)
		{
			Destroy(gameObject);
			return;
		}

		properNouns.Add("card reader");
	}

	void OnEnable()
	{
		PublishInput.EmitPublisher += Parse;
	}

	void OnDisable()
	{
		PublishInput.EmitPublisher -= Parse;
	}

	bool restart = false;
	float startTime = 0f;
	float delay = 0;
	private void Update()
	{
		

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

	}

	public void RestartDelay(float delayTime)
	{

        endingDeath.StartDeath(3f);
        Invoke("Restart", delayTime);

	}

	public void Restart()
	{
        //Number of the game scene
       


        SceneManager.LoadScene(0);
		//Reloads the game scene triggering the intro and reseting everything. Destroy this game object first so it dumps all state information and gets a new GameManager.
		//Destroy(gameObject);
	}


    private void Reload()
    {

    }
	public void AddNoun(string name)
	{
		properNouns.Add(name);
	}

	public void AddEvent(Event e)
	{
		if (!events.ContainsKey(e.eventName))
		{
			events.Add(e.eventName, e);
		}
	}

	static private string[] KEYWORDS_DIRECTIONS = { "up", "down", "left", "right", "north", "south", "east", "west" };
	static private string[] KEYWORDS_ROOM_NAMES = {};
	static private string[] KEYWORDS_CANCEL = { "nevermind", "never", "clear", "cancel" };

	// I don't know if this makes your life easier. But it could lead to way fewer of these nested if blocks
	static private Regex moveExpr = new Regex(@"^(move|go|head)\s?(to)?\s?(the)?(?<destination>.*$)");
	static private Regex cancelExpr = new Regex(@"(never\s?mind|clear|cancel|ignore)");
	static private Regex takeExpre = new Regex(@"(take|collect|pick up)\s?(a|the|one)?\s?(?<item>.*$)");


	static private Regex lever = new Regex(@"(pull|toggle|switch|user)\s?(?<leverPosition>right|middle|center|left)\s?(?<leverState>on|off)");

	static private string[] KEYWORDS_VERB_IGNORE_WORDS = { "at", "with", "to", "the", "a" };

	public bool tryPickupItem(Interactable item) {

		if (pc.roomOccupied.Take(item))
		{
			audioSource.PlayAudioOnce(ref pickUp, AudioType.SoundEffect);
			pc.inventory.Add(item);
			return true;
		}
		return false;
	}

	public void Parse(string inputText)
	{
		inputText = CorrectMisheard(inputText);

		string[] input = inputText.ToLower().Split(' ');

		input = CombineNames(input);

		input = input.Concat(new string[] { "", "", "", "", "", "", "", "", "", "", ""}).ToArray(); // what?

		Debug.Log("Parsing: " + inputText);
		if (cancelExpr.Match(inputText).Success) { // shortcut
			Debug.Log("canceled");
			return;
		}

		var destinationMatch = moveExpr.Match(inputText);
		if (destinationMatch.Success)
		{
			var destinationString = destinationMatch?.Groups["destination"]?.Value;
			pc.giveMoveInstruction(destinationString); // See?
			return;
		}


		var takeMatch = takeExpre.Match(inputText);
		if (takeMatch.Success)
		{
			var takeByName = takeMatch?.Groups["item"]?.Value;
			Debug.Log(takeByName);

			var itemToTake = pc.roomOccupied.GetInteractables()
				.FirstOrDefault(i => {
					return i.nameString == takeByName;
				});
			if (tryPickupItem(itemToTake))
			{
				return;
			}
		}

		//Observation
		if (input[0] == "examine" || input[0] == "inspect")
		{
			//test to see if input[1] is in the room or inventory. if so call it's examine property
			//Probably need to have objects that return a .examine property.
			if (IsAccessible(input[1]))
			{
				//examine that item'
				Debug.Log("examine item " + input[1]);

				audioSource.PlayAudioOnce(ref GetAccessibleByName(input[1]).description, AudioType.VoiceOver);
			}
			else
			{
				//examine the room
				audioSource.PlayAudioOnce(ref pc.roomOccupied.longDescription, AudioType.VoiceOver);

				string temp = "";

				foreach (Interactable i in pc.roomOccupied.GetInteractables())

				{
					temp += i.nameString + ", ";
					audioSource.PlayAudioOnce(ref i.name, AudioType.VoiceOver);
				}

				Debug.Log(temp);
			}

		}
		else if (input[0] == "where")
		{
			if (input[1] == "is")
			{
				//Maybe look to see if the item in input 2 exists and tell them what room it's in?
			}
			else if (input[1] == "am")
			{
				//Assume it's "where am I?" Return the room description. (Breif)
			}
		}
		else if (input[0] == "look")
		{
			if (input[1] == "at")
			{
				//Call object input[2]'s look function probably same as examine
			}
			else
			{
				//just look at the room. List contents.
			}
		}
		else if ((input[0] == "what" && input[1] == "am" && input[2] == "i" && input[3] == "doing") || (input[0] == "what" && input[1] == "should" && input[2] == "i" && input[3] == "do") || (input[0] == "what" && input[1] == "is" && input[2] == "my" && input[3] == "objective"))
		{
			//read next code sound
			Debug.Log("the next code!");

			if (currentCode < codeClips.Length)
			{
				audioSource.PlayAudioOnce(ref codeClips[currentCode], AudioType.VoiceOver);
			}
			
		}
		//Interaction
		//else if (input[0] == "interact" || input[0] == "use")
		//{
		//	string target = "";

		//	if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[1]))
		//	{
		//		//Call the default interaction of whatever input[2] is if it is accessible.
		//		target = input[2];
		//	}
		//	else
		//	{
		//		//See if input[1] is an interactable and it is accessible call its default interact
		//		target = input[1];
		//	}

		//	bool checkRoom = true;

		//	foreach (Interactable i in pc.inventory)
		//	{
		//		if (i.nameString == target)
		//		{
		//			i.Interact("");
		//			checkRoom = false;
		//			break;
		//		}
		//	}

		//	if (checkRoom)
		//	{
		//		foreach (Interactable i in pc.roomOccupied.GetInteractables())
		//		{
		//			if (i.nameString == target)
		//			{
		//				i.Interact("");
		//				break;
		//			}
		//		}
		//	}
			
		//}
		else if ((input[0] == "quit" || input[0] == "end" || input[0] == "exit") && input[1] == "game")
		{
			Application.Quit();
		}
		else
		{
			//play with waste atomizer

			//play with the waste atomizer

			//Shoot laser at terminal
			//0 - shoot - verb
			//1 - laser - target
			//2 - at - ignore word
			//3 - terminal - target2

			//Shoot the laser at terminal
			//0 - shoot - verb
			//1 - the - ignore word
			//2 - laser - target
			//3 - at - ignore word
			//4 - terminal - target2

			//Shoot the laser at the terminal
			//0 - shoot - verb
			//1 - the - ignore word
			//2 - laser - target
			//3 - at - ignore word
			//4 - the - ignore word
			//5 - terminal - target2

			//Shoot laser at the terminal
			//0 - shoot - verb
			//1 - laser - target
			//2 - at - ignore word
			//3 - the - ignore word
			//4 - terminal - target2

			bool foundTarget = false;

			string target = "";
			string target2 = "";

			Debug.Log("0: " + input[0] + " 1: " + input[1] + " 2: " + input[2] + " 3: " + input[3] + " 4: " + input[4] + " 5: " + input[5]);

			if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[1]))
			{
				target = input[2];

				if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[3]))
				{
					if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[4]))
					{
						target2 = input[5];
					}
					else
					{
						target2 = input[4];
					}
				}
			}
			else
			{
				target = input[1];

				if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[2]))
				{
					if (KEYWORDS_VERB_IGNORE_WORDS.Contains(input[3]))
					{
						target2 = input[4];
					}
					else
					{
						target2 = input[3];
					}
				}
			}

			Debug.Log("target2: " + target2);

			//look for generic verbs that are accessible to me in the room.
			foreach (Interactable i in pc.roomOccupied.GetInteractables())
			{
				if (i.verbs.ContainsKey(input[0]))
				{
					if (i.nameString == target)
					{
						Debug.Log("target 2 " + target2);
						if (target2 != "")
						{
							if (IsAccessible(target2))
							{
								i.Interact(input[0], target2);
							}
							else
							{
								//voice line for I can't find that/target 2
								Debug.Log("I can't find " + target2);
							}
						}
						else
						{
							i.Interact(input[0]);
						}

						foundTarget = true;
						break;
					}
				}
			}

			if (!foundTarget)
			{
				//look for generic verbs that are accessible to me in inventory.
				foreach (Interactable i in pc.inventory)
				{
					if (i.verbs.ContainsKey(input[0]))
					{
						if (i.nameString == target)
						{
							Debug.Log("trying inventory target 2 " + target2);
							if (target2 != "")
							{
								if (IsAccessible(target2))
								{
									i.Interact(input[0], target2);
								}
								else
								{
									//voice line for I can't find that/target 2
									Debug.Log("I can't find " + target2);
								}
							}
							else
							{
								i.Interact(input[0]);
							}

							foundTarget = true;
							break;
						}
					}
				}
			}

			if (!foundTarget)
			{
				//I can't do that here.
				Debug.Log("I can't do that here.");
			}
		}
	}

	private string CorrectMisheard(string input)
	{
		Dictionary<string, string> words = new Dictionary<string, string> { { "atomiser", "atomizer" }, { "waist", "waste" }, { "atomizing", "atomizer" }, { "uski", "use key" }, { "swype", "swipe"} };

		foreach (string w in words.Keys)
		{
			input = input.Replace(w, words[w]);
		}

		return input;
	}

	private string[] CombineNames(string[] input)
	{
		List<string> temp = input.ToList<string>();
		List<string> result = new List<string>();


		int count = 0;
		bool skipNextWord = false;
		foreach (string word in temp)
		{
			Debug.Log(word);
			bool foundName = false;

			if (skipNextWord)
			{
				skipNextWord = false;
				continue;
			}

			foreach (string noun in properNouns)
			{
				string[] splitNoun = noun.Split(' ');
				try
				{
					if (word == splitNoun[0] && temp[count + 1] == splitNoun[1])
					{
						result.Add(noun);
						skipNextWord = true;
						foundName = true;
						break;
					}
				}
				catch
				{

				}
			}

			if (!foundName)
			{
				result.Add(word);
			}

			count++;
		}

		return result.ToArray();

	}

	private bool IsAccessible(string name)
	{
		bool foundTarget = false;

		//look for generic verbs that are accessible to me in the room.
		foreach (Interactable i in pc.roomOccupied.GetInteractables())
		{
			if (i.nameString == name)
			{
				foundTarget = true;
				break;
			}
		}

		if (!foundTarget)
		{
			//look for generic verbs that are accessible to me in inventory.
			foreach (Interactable i in pc.inventory)
			{
				if (i.nameString == name)
				{
					foundTarget = true;
					break;
				}
			}
		}

		return foundTarget;
	}

	private Interactable GetAccessibleByName(string name)
	{
		Interactable foundTarget = null;

		//look for generic verbs that are accessible to me in the room.
		foreach (Interactable i in pc.roomOccupied.GetInteractables())
		{
			if (i.nameString == name)
			{
				foundTarget = i;
				break;
			}
		}

		if (!foundTarget)
		{
			//look for generic verbs that are accessible to me in inventory.
			foreach (Interactable i in pc.inventory)
			{
				if (i.nameString == name)
				{
					foundTarget = i;
					break;
				}
			}
		}

		return foundTarget;
	}

}
