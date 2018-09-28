using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility;

[System.Flags]
public enum Scenario
{
	None = 0,
	A = 1,
	B = 2,
	C = 4
}

[System.Serializable]
public class GameEvent
{
	public string name;
	public int ID;
	public bool useSubEventMessages;
	public string[] startingMessage = new string[0];
	public string[] endingMessage = new string[0];
	public Scenario scenario;
	public DynamicValue oddsOfOccuring = new DynamicValue();

	public DynamicValue MaxOccurencePerPlaythrough = new DynamicValue();
	[MinMaxSlider(0, 30)] public Vector2Int Occurence;
	[MinMaxSlider(0, 10)] public Vector2Int Duration;

	public GameValueModifer[] modifers = new GameValueModifer[0];
	public SubEvent[] SubEvents = new SubEvent[0];

	public bool occuring;
	public int occuredTimes = 0;
	public int occurationStartDate;

	public event Action<GameEvent> OnEventOccuring;
	public event Action<GameEvent> OnEventStoping;

	public GameEvent() { }
	public GameEvent(GameEvent gEvent)
	{
		name = gEvent.name;
		ID = gEvent.ID;
		useSubEventMessages = gEvent.useSubEventMessages;
		scenario = gEvent.scenario;
		oddsOfOccuring = new DynamicValue(gEvent.oddsOfOccuring);
		MaxOccurencePerPlaythrough = new DynamicValue(gEvent.MaxOccurencePerPlaythrough);
		Occurence = gEvent.Occurence;
		Duration = gEvent.Duration;

		startingMessage = new string[gEvent.startingMessage.Length];
		for (int i = 0; i < startingMessage.Length; i++)
		{
			startingMessage[i] = gEvent.startingMessage[i];
		}

		endingMessage = new string[gEvent.endingMessage.Length];
		for (int i = 0; i < endingMessage.Length; i++)
		{
			endingMessage[i] = gEvent.endingMessage[i];
		}

		modifers = new GameValueModifer[gEvent.modifers.Length];
		for (int i = 0; i < modifers.Length; i++)
		{
			modifers[i] = new GameValueModifer(gEvent.modifers[i]);
		}

		SubEvents = new SubEvent[gEvent.SubEvents.Length];
		for (int i = 0; i < SubEvents.Length; i++)
		{
			SubEvents[i] = new SubEvent(gEvent.SubEvents[i]);
		}
	}

	public void CheckEvent()
	{
		if (occuring && CanStop())
		{
			Stop();
		}

		if (!occuring)
		{
			occuring = TryInvoke();
			if (CanStop())
				occuring = false;
		}
	}

	bool TryInvoke()
	{
		if (occuredTimes >= MaxOccurencePerPlaythrough ||
			!Occurence.IsIncluded(TimeManager.Date) ||
			Random.value > oddsOfOccuring)
			return false;

		Debug.Log("Event Occuring, Start date " + TimeManager.Date);
		if (OnEventOccuring != null)
			OnEventOccuring.Invoke(this);

		occurationStartDate = TimeManager.Date;
		occuredTimes++;
		for (int i = 0; i < modifers.Length; i ++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (Duration == Vector2Int.zero)
				GameDataManager.GameValues.MergeModifier(modifers[i]);
			else
				GameDataManager.GameValues.AddModifier(modifers[i]);
		}
		
		var chanceOnce = SubEvents.Select((x) => { return x.occuringMethod == SubEvent.OccuringMethod.Chance_Once; });
		var chanceSum = 0f;
		var chanceNum = 0f;

		for (int i = 0; i < SubEvents.Length; i ++)
		{
			if (!chanceOnce.ElementAt(i) || SubEvents[i].isExecuting) continue;
			chanceSum += SubEvents[i].chance;
		}

		chanceNum = Random.value * chanceSum;
		chanceSum = 0;
		for (int i = 0; i < SubEvents.Length; i++)
		{
			if (SubEvents[i].occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Once ||
				SubEvents[i].occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Multiple)
				continue;
			if (chanceOnce.ElementAt(i))
			{
				chanceSum += SubEvents[i].chance;
				if (chanceSum > chanceNum)
				{
					Debug.Log("Subevent " + i + " is executing");
					SubEvents[i].Execute();
					chanceNum = float.PositiveInfinity;
				}
				continue;
			}
			
			if (Random.value < SubEvents[i].chance)
			{
				SubEvents[i].Execute();
			}
		}

		return true;
	}

	bool CanStop()
	{
		foreach (var se in SubEvents)
		{
			if (se.isExecuting && !se.CanStop(occurationStartDate, TimeManager.Date))
			{
				Debug.Log(se.Duration);
				return false;
			}
		}

		if (Duration == Vector2Int.zero) return true;
		if (Duration.RandomBetween() > (TimeManager.Date - occurationStartDate))
			return false;

		return true;
	}

	void Stop()
	{
		Debug.Log("Event Stoping, ending date: " + TimeManager.Date);
		if (OnEventStoping != null)
			OnEventStoping.Invoke(this);
		for (int i = 0; i < modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(modifers[i]);
		}

		var chanceOnce = SubEvents.Select((x) => { return x.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Once; });
		var chanceSum = 0f;
		var chanceNum = 0f;

		for (int i = 0; i < SubEvents.Length; i++)
		{
			if (!chanceOnce.ElementAt(i) || SubEvents[i].isExecuting) continue;
			chanceSum += SubEvents[i].chance;
		}

		chanceNum = Random.value * chanceSum;
		chanceSum = 0;

		for (int i = 0; i < SubEvents.Length; i++)
		{
			if (SubEvents[i].occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Once ||
				SubEvents[i].occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Multiple)
			{
				if (SubEvents[i].isExecuting)
					SubEvents[i].Stop();
				continue;
			}

			if (chanceOnce.ElementAt(i))
			{
				chanceSum += SubEvents[i].chance;
				if (chanceSum > chanceNum)
				{
					Debug.Log("Subevent " + i + " is executing");
					SubEvents[i].Execute();
					chanceNum = float.PositiveInfinity;
				}
				continue;
			}

			if (Random.value < SubEvents[i].chance)
			{
				SubEvents[i].Execute();
			}
		}

		occuring = false;
	}
}

[System.Serializable]
public class SubEvent
{
	public enum OccuringMethod
	{
		Chance_Once,
		Chance_Multiple,
		AtTheEnd_Once,
		AtTheEnd_Multiple,
	}

	public OccuringMethod occuringMethod;
	public DynamicValue chance = new DynamicValue();
	[MinMaxSlider(0, 20)] public Vector2Int Duration;
	public GameValueModifer[] modifers = new GameValueModifer[0];
	public string[] startingMessage = new string[0];
	public string[] endingMessage = new string[0];
	public bool isExecuting;

	public SubEvent() { }

	public SubEvent(SubEvent sEvent)
	{
		occuringMethod = sEvent.occuringMethod;
		chance = new DynamicValue(sEvent.chance);
		Duration = sEvent.Duration;

		startingMessage = new string[sEvent.startingMessage.Length];
		for (int i = 0; i < startingMessage.Length; i++)
		{
			startingMessage[i] = sEvent.startingMessage[i];
		}

		endingMessage = new string[sEvent.endingMessage.Length];
		for (int i = 0; i < endingMessage.Length; i++)
		{
			endingMessage[i] = sEvent.endingMessage[i];
		}

		modifers = new GameValueModifer[sEvent.modifers.Length];
		for (int i = 0; i < modifers.Length; i++)
		{
			modifers[i] = sEvent.modifers[i];
		}
	}

	public void Execute()
	{
		for (int i = 0; i < modifers.Length; i++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (Duration == Vector2Int.zero)
				GameDataManager.GameValues.MergeModifier(modifers[i]);
			else
			{
				GameDataManager.GameValues.AddModifier(modifers[i]);
				isExecuting = true;
			}
		}
	}

	public bool CanStop(int startingDate, int currentDate)
	{
		if (Duration == Vector2Int.zero) return true;
		if (Duration.RandomBetween() <= currentDate - startingDate) return true;
		return false;
	}

	public void Stop()
	{
		if (Duration == Vector2.zero) return;

		for (int i = 0; i < modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(modifers[i]);
		}
		isExecuting = false;
	}
}
