using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

public class RuntimeEvent
{
	public readonly int ID;
	public GameEvent eventRef;
	
	public bool occuring;
	public int occuredTimes = 0;
	public int occurationStartDate;
	public int occurationEndDate;

	public RuntimeSubEvent[] subEvents;

	public event Action<RuntimeEvent> OnEventOccuring;
	public event Action<RuntimeEvent> OnEventStoping;

	public RuntimeEvent(int id)
	{
		if (id < 0 || id >= GameDatabase.Instance.eventList.Count)
			throw new System.ArgumentOutOfRangeException();

		ID = id;
		eventRef = GameDatabase.Instance.eventList[id];

		subEvents = new RuntimeSubEvent[eventRef.subEvents.Length];
		for (int i = 0; i < subEvents.Length; i ++)
		{
			subEvents[i] = new RuntimeSubEvent(eventRef.subEvents[i]);
		}
	}

	public void CheckEvent()
	{
		if (!occuring && eventRef.StartingCondition.IsSatisfied())
		{
			occuring = TryInvoke();
			if (occuring && CanStop())
				occuring = false;
		}

		if (!eventRef.ContinualCondition.IsSatisfied())
			Abort();

		foreach (var se in subEvents)
		{
			if (se.isExecuting &&
				!se.eventRef.ContinualCondition.IsSatisfied())
				se.Stop();
		}

		if (occuring && CanStop())
		{
			TryStop();
		}
	}

	bool TryInvoke()
	{
		// If this event is invoked enough times, or out of proper invoking date range, 
		// or fate(random algorithm) haven't decide yet, return from here.
		if (occuredTimes >= eventRef.MaxOccurencePerPlaythrough ||
			!eventRef.Occurence.IsIncluded(TimeManager.Date) ||
			Random.value > eventRef.oddsOfOccuring)
			return false;

		Debug.Log("Event Occuring, Start date " + TimeManager.Date);

		occurationStartDate = TimeManager.Date;
		occurationEndDate = occurationStartDate + eventRef.Duration.RandomBetween();
		occuredTimes++;
		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (eventRef.Duration == Vector2Int.zero)
				GameDataManager.GameValues.MergeModifier(eventRef.modifers[i]);
			else
				GameDataManager.GameValues.AddModifier(eventRef.modifers[i]);
		}

		// Calculate occurence of sub events with occuring method of chose one
		var chanceSum = 0f;
		var chanceNum = 0f;

		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].eventRef.occuringMethod != SubEvent.OccuringMethod.Chose_One || 
				subEvents[i].isExecuting ||
				!subEvents[i].eventRef.StartingCondition.IsSatisfied())
				continue;
			chanceSum += subEvents[i].eventRef.chance;
		}

		Debug.Log("Event " + ID + ", Chance sum " + chanceSum);
		chanceNum = Random.value * chanceSum;
		chanceSum = 0;

		// Going through all the sub events
		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_One ||
				subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Multiple)
				continue;

			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.Chose_One)
			{
				if (chanceSum >= chanceNum ||
					!subEvents[i].eventRef.StartingCondition.IsSatisfied())
					continue;

				chanceSum += subEvents[i].eventRef.chance;
				if (chanceSum > chanceNum)
				{
					subEvents[i].Execute();
				}
			}
			else 
			if (Random.value < subEvents[i].eventRef.chance &&
				subEvents[i].eventRef.StartingCondition.IsSatisfied())
			{
				subEvents[i].Execute();
			}
		}

		// Invoke occuring event
		if (OnEventOccuring != null)
			OnEventOccuring.Invoke(this);

		return true;
	}

	bool CanStop()
	{
		if (!eventRef.ContinualCondition.IsSatisfied())
			return true;

		foreach (var se in subEvents)
		{
			if (se.isExecuting && !se.CanStop())
			{
				Debug.Log(se.eventRef.Duration);
				return false;
			}
		}
		
		if (occurationEndDate > TimeManager.Date)
			return false;

		return true;
	}

	void TryStop()
	{
		Debug.Log("Event Stoping, ending date: " + TimeManager.Date);
		if (OnEventStoping != null)
			OnEventStoping.Invoke(this);

		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(eventRef.modifers[i]);
		}
		
		var chanceSum = 0f;
		var chanceNum = 0f;

		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].eventRef.occuringMethod != SubEvent.OccuringMethod.AtTheEnd_One || 
				subEvents[i].isExecuting ||
				!subEvents[i].eventRef.StartingCondition.IsSatisfied())
				continue;
			chanceSum += subEvents[i].eventRef.chance;
		}

		chanceNum = Random.value * chanceSum;
		chanceSum = 0;

		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_One)
			{
				if (chanceSum >= chanceNum ||
					!subEvents[i].eventRef.StartingCondition.IsSatisfied())
					continue;

				chanceSum += subEvents[i].eventRef.chance;
				if (chanceSum > chanceNum)
				{
					Debug.Log("Subevent " + i + " is executing");
					subEvents[i].Execute();
					chanceNum = float.PositiveInfinity;
				}
			}
			else if (
				subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Multiple &&
				Random.value < subEvents[i].eventRef.chance &&
				subEvents[i].eventRef.StartingCondition.IsSatisfied())
			{
				subEvents[i].Execute();
			}
		}

		// If ending event is executing, we cannot stop this event now.
		foreach (var se in subEvents)
		{
			if (se.eventRef.occuringMethod == SubEvent.OccuringMethod.Chose_One ||
				se.eventRef.occuringMethod == SubEvent.OccuringMethod.Chose_Multiple)
				continue;

			if (se.isExecuting && !se.CanStop())
			{
				return;
			}
		}

		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].isExecuting)
				subEvents[i].Stop();
		}

		occuring = false;
	}

	void Abort()
	{
		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(eventRef.modifers[i]);
		}

		foreach (var se in subEvents)
		{
			if (se.isExecuting)
				se.Stop();
		}

		occuring = false;
	}
}

public class RuntimeSubEvent
{
	public readonly SubEvent eventRef;

	public int occurationStartDate = -1;
	public int occurationEndDate = -1;
	public bool isExecuting;

	public RuntimeSubEvent(SubEvent @event)
	{
		if (@event == null)
			throw new System.ArgumentNullException();

		eventRef = @event;
	}

	public void Execute()
	{
		Debug.Log(eventRef.label + " is executing");
		
		occurationStartDate = TimeManager.Date;
		occurationEndDate = eventRef.Duration.RandomBetween() + occurationStartDate;
		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (eventRef.Duration == Vector2Int.zero)
			{
				GameDataManager.GameValues.MergeModifier(eventRef.modifers[i]);
			}
			else
			{
				GameDataManager.GameValues.AddModifier(eventRef.modifers[i]);
				isExecuting = true;
			}
		}
	}

	public bool CanStop()
	{
		if (!eventRef.ContinualCondition.IsSatisfied())
			return true;

		return occurationEndDate <= TimeManager.Date;
	}

	public void Stop()
	{
		if (eventRef.Duration == Vector2.zero) return;

		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(eventRef.modifers[i]);
		}
		isExecuting = false;
	}
}
