using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaderEvent : Event
{


	public override void triggerEvent()
	{
		if (GameManager.instance.pc.roomOccupied.roomName == "hall")
		{
			base.triggerEvent();

			GameManager.instance.states["bridgecard"] = "unlocked";
		}
		else
		{
			Debug.Log("You need to be in the same room as the card reader");
		}
		
	}
}
