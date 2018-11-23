using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class EventManager : MonoSingleton<EventManager>
{
	public int starvationEventIndexOffset = 5;
	public int randomDeathEventIndexOffset = 9;
	public int taxEvasionEventIndex = 13;

	public int maxTaxEvasionTimes = 8;
	public int taxEvasionTimes = 0;

	public List<RuntimeEvent> EventList;

	[SerializeField] FadeIn GoodEnding;
	[SerializeField] FadeIn BadEnding;
	[SerializeField] FadeIn DiedEnding;

	protected override void Awake()
	{
		base.Awake();
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

		StarvationCheck();
		RandomDeathCheck();
		TaxEvasionEventCheck();
		CheckGameOver();
		GameDataManager.UpdateValues();
	}

	private void CheckGameOver()
	{
		if (FamilyManager.FamilyMembers[0].gone)
		{
			DiedEnding.StartFadein();
			return;
		}

		if (TimeManager.Date >= GameDataManager.MaxWeek)
		{
			if (CheckList.Instance.IsCheckListFinished())
			{
				GoodEnding.StartFadein();
			}
			else
			{
				BadEnding.StartFadein();
			}
		}
	}

	private void RandomDeathCheck()
	{
		var list = FamilyManager.FamilyMembers;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gone) continue;
			var deathRate = list[i].dyingRate + (100 - list[i].mentalHealth) * list[i].mentalDyingRatio;
			if (Random.value > deathRate) continue;

			list[i].gone = true;
			GameDataManager.GameValues[GameValueType.Fertiliser]++;
			GameDataManager.GameValues.CalculateModifiedValue();
			var title = EventList[i + randomDeathEventIndexOffset].eventRef.name;
			var message = EventList[i + randomDeathEventIndexOffset].eventRef.startingMessage[
				Random.Range(0, EventList[i + randomDeathEventIndexOffset].eventRef.startingMessage.Length)];
			MessageBox.DisplayMessage(title, message);
		}
	}

	private void TaxEvasionEventCheck()
	{
		if (taxEvasionTimes < maxTaxEvasionTimes) return;

		var list = FamilyManager.FamilyMembers;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gone) continue;

			list[i].gone = true;
			GameDataManager.GameValues[GameValueType.Fertiliser]++;
			GameDataManager.GameValues.CalculateModifiedValue();
			print(GameDataManager.GameValues[GameValueType.Fertiliser]);
		}

		var title = EventList[taxEvasionEventIndex].eventRef.name;
		var message = EventList[taxEvasionEventIndex].eventRef.startingMessage[
			Random.Range(0, EventList[taxEvasionEventIndex].eventRef.startingMessage.Length)];
		MessageBox.DisplayMessage(title, message);
	}

	private void StarvationCheck()
	{
		var list = FamilyManager.FamilyMembers;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].gone) continue;
			if (list[i].hunger < 1) continue;
			if (list[i].hunger < 2)
			{
				list[i].mentalHealth -= 10;
				Mathf.Clamp(list[i].mentalHealth, 0, 100);
				continue;
			}

			list[i].gone = true;
			GameDataManager.GameValues[GameValueType.Fertiliser]++;
			GameDataManager.GameValues.CalculateModifiedValue();
			var title = EventList[i + starvationEventIndexOffset].eventRef.name;
			var message = EventList[i + starvationEventIndexOffset].eventRef.startingMessage[
				Random.Range(0, EventList[i + starvationEventIndexOffset].eventRef.startingMessage.Length)];
			MessageBox.DisplayMessage(title, message);
		}
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
