using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverEvent : Event
{
	public bool state = false;


	public override void triggerEvent()
	{
		

		state = !state;

		char newNum = '0';

		if (state)
		{
			newNum = '1';
		}
		else
		{
			newNum = '0';
		}

		string temp = GameManager.instance.states["levers"];

		if (eventName == "leverone")
		{
			temp = newNum + temp.ToCharArray()[1].ToString() + temp.ToCharArray()[2].ToString();
		}
		else if (eventName == "levertwo")
		{
			temp = temp.ToCharArray()[0].ToString() + newNum + temp.ToCharArray()[2].ToString();
		}
		else if (eventName == "leverthree")
		{
			temp = temp.ToCharArray()[0].ToString() + temp.ToCharArray()[1].ToString() + newNum;
		}

		GameManager.instance.states["levers"] = temp;

		Debug.Log(eventName + " Lever status: " + GameManager.instance.states["levers"]);

		if (GameManager.instance.states["levers"] == "101")
		{
			//Unlock engine room (disable levers)
			GameManager.instance.states["enginedoor"] = "unlocked";
			base.triggerEvent();
		}
		else if(GameManager.instance.states["levers"] == "111")
		{
			GameManager.instance.events["atomizer"].triggerEvent();
		}
	}
}
