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
		foreach (var e in Database.Instance.eventList)
		{
			EventList.Add(new RuntimeEvent(e.ID));
		}

		for (int i = 0; i < EventList.Count; i++)
		{
			EventList[i].OnEventOccuring += EventManager_OnEventOccuring;
			EventList[i].OnEventStoping += EventManager_OnEventStoping;
		}
		CheckEvents();
	}

	public void CheckEvents()
	{
		print("CheckEvent");
		for (int i = 0; i < EventList.Count; i++)
		{
			EventList[i].CheckEvent();
		}
		GameDataManager.UpdateValues();
	}

	private void EventManager_OnEventStoping(RuntimeEvent ge)
	{
		int index;
		string message;

		Debug.Log(ge.eventRef.name + " Ending");
		if (ge.eventRef.useSubEventMessages)
		{
			foreach (var se in ge.subEvents)
			{
				if (!se.isExecuting) continue;
				if (se.eventRef.endingMessage.Length <= 0)
					continue;
				index = Random.Range(0, se.eventRef.endingMessage.Length);
				message = se.eventRef.endingMessage[index];
				MessageBox.DisplayMessage(se.eventRef.label, message);
			}
		}
		else
		{
			index = Random.Range(0, ge.eventRef.endingMessage.Length);
			message = ge.eventRef.endingMessage[index];
			MessageBox.DisplayMessage(ge.eventRef.name, message);
		}
	}

	private void EventManager_OnEventOccuring(RuntimeEvent ge)
	{
		int index;
		string message;

		Debug.Log(ge.eventRef.name + " Starting");

		if (ge.eventRef.useSubEventMessages)
		{
			foreach (var se in ge.subEvents)
			{
				if (se.occurationStartDate != TimeManager.Date) continue;
				if (se.eventRef.startingMessage.Length <= 0)
					continue;
				index = Random.Range(0, se.eventRef.startingMessage.Length);
				message = se.eventRef.startingMessage[index];
				MessageBox.DisplayMessage(se.eventRef.label, message);
			}
		}
		else
		{
			index = Random.Range(0, ge.eventRef.startingMessage.Length);
			message = ge.eventRef.startingMessage[index];
			MessageBox.DisplayMessage(ge.eventRef.name, message);
		}
	}
}
