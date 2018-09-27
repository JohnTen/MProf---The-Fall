using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class EventManager : MonoSingleton<EventManager>
{
	public List<GameEvent> EventList;

	protected override void Awake()
	{
		base.Awake();
		//TimeManager.OnTimePassed += TimeManager_OnTimePassed;
		EventList = new List<GameEvent>();
		foreach (var e in Database.Instance.eventList)
		{
			EventList.Add(new GameEvent(e));
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

	private void EventManager_OnEventStoping(GameEvent ge)
	{
		int index;
		string message;

		Debug.Log(ge.name + " Ending");
		if (ge.useSubEventMessages)
		{
			foreach (var se in ge.SubEvents)
			{
				if (!se.isExecuting) continue;
				if (se.endingMessage.Length <= 0)
					continue;
				index = Random.Range(0, se.endingMessage.Length);
				message = se.endingMessage[index];
				MessageBox.DisplayMessage(ge.name, message);
			}
		}
		else
		{
			index = Random.Range(0, ge.endingMessage.Length);
			message = ge.endingMessage[index];
			MessageBox.DisplayMessage(ge.name, message);
		}
	}

	private void EventManager_OnEventOccuring(GameEvent ge)
	{
		int index;
		string message;

		Debug.Log(ge.name + " Starting");

		if (ge.useSubEventMessages)
		{
			foreach (var se in ge.SubEvents)
			{
				if (!se.isExecuting) continue;
				if (se.startingMessage.Length <= 0)
					continue;
				index = Random.Range(0, se.startingMessage.Length);
				message = se.startingMessage[index];
				MessageBox.DisplayMessage(ge.name, message);
				break;
			}
		}
		else
		{
			index = Random.Range(0, ge.startingMessage.Length);
			message = ge.startingMessage[index];
			MessageBox.DisplayMessage(ge.name, message);
		}
	}
}
