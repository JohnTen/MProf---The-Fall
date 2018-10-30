using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class EventManager : MonoSingleton<EventManager>
{
	public List<RuntimeEvent> EventList;

	protected override void Awake()
	{
		base.Awake();
		//TimeManager.OnTimePassed += TimeManager_OnTimePassed;
		EventList = new List<RuntimeEvent>();
		foreach (var e in GameDatabase.Instance.eventList)
		{
			EventList.Add(new RuntimeEvent(e.ID));
		}

		for (int i = 0; i < EventList.Count; i++)
		{
			EventList[i].OnEventOccuring += EventManager_OnEventOccuring;
			EventList[i].OnEventStoping += EventManager_OnEventStoping;
			EventList[i].OnSubEventOccuring += EventManager_OnSubEventOccuring;
			EventList[i].OnSubEventStoping += EventManager_OnSubEventStoping;
		}
		CheckEvents();
	}

	public void CheckEvents()
	{
		for (int i = 0; i < EventList.Count; i++)
		{
			EventList[i].CheckEvent();
		}
		GameDataManager.UpdateValues();
	}

	private void EventManager_OnSubEventStoping(RuntimeEvent ge, RuntimeSubEvent se)
	{
		if (!ge.eventRef.useSubEventMessages) return;
		int index;
		string message;

		Debug.Log(se.eventRef.label + " Ending");

		index = Random.Range(0, se.eventRef.endingMessage.Length);
		message = se.eventRef.endingMessage[index];
		MessageBox.DisplayMessage(se.eventRef.label, message);
	}

	private void EventManager_OnSubEventOccuring(RuntimeEvent ge, RuntimeSubEvent se)
	{
		if (!ge.eventRef.useSubEventMessages) return;
		int index;
		string message;

		Debug.Log(se.eventRef.label + " Starting");

		index = Random.Range(0, se.eventRef.startingMessage.Length);
		message = se.eventRef.startingMessage[index];
		MessageBox.DisplayMessage(se.eventRef.label, message);
	}

	private void EventManager_OnEventStoping(RuntimeEvent ge)
	{
		if (ge.eventRef.useSubEventMessages) return;
		int index;
		string message;

		Debug.Log(ge.eventRef.name + " Ending");
		
		index = Random.Range(0, ge.eventRef.endingMessage.Length);
		message = ge.eventRef.endingMessage[index];
		MessageBox.DisplayMessage(ge.eventRef.name, message);
	}

	private void EventManager_OnEventOccuring(RuntimeEvent ge)
	{
		if (ge.eventRef.useSubEventMessages) return;
		int index;
		string message;

		Debug.Log(ge.eventRef.name + " Starting");
		
		index = Random.Range(0, ge.eventRef.startingMessage.Length);
		message = ge.eventRef.startingMessage[index];
		MessageBox.DisplayMessage(ge.eventRef.name, message);
	}
}
