using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

[System.Serializable]
public class RuntimeEvent
{
	public readonly int ID;
	public GameEvent eventRef;
	
	public bool occuring;
	public int occuredTimes = 0;
	public int occurationStartDate = -1;
	public int occurationEndDate = -1;

	public RuntimeSubEvent[] subEvents;

	public event Action<RuntimeEvent> OnEventOccuring;
	public event Action<RuntimeEvent> OnEventStoping;
	public event Action<RuntimeEvent, RuntimeSubEvent> OnSubEventOccuring;
	public event Action<RuntimeEvent, RuntimeSubEvent> OnSubEventStoping;

	public bool IsReachedEndingDate
	{
		get { return occurationEndDate <= TimeManager.Date; }
	}

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

		Debug.Log(eventRef.name + " Occuring, Start date " + TimeManager.Date);

		// Invoke occuring event
		if (OnEventOccuring != null)
			OnEventOccuring.Invoke(this);

		// Update occuring data
		occurationStartDate = TimeManager.Date;
		occurationEndDate = occurationStartDate + eventRef.Duration.RandomBetweenIncluded();
		occuredTimes++;

		// Apply modifiers
		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (eventRef.Duration == Vector2Int.zero)
				GameDataManager.GameValues.MergeModifier(eventRef.modifers[i]);
			else
				GameDataManager.GameValues.AddModifier(eventRef.modifers[i]);
		}

		// Calculate occuring chance of sub events with occuring method of chose one
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
				if (chanceSum >= chanceNum)
				{
					subEvents[i].Execute();
					if (OnSubEventOccuring != null)
						OnSubEventOccuring.Invoke(this, subEvents[i]);
				}
			}
			else 
			if (Random.value < subEvents[i].eventRef.chance &&
				subEvents[i].eventRef.StartingCondition.IsSatisfied())
			{
				subEvents[i].Execute();
				if (OnSubEventOccuring != null)
					OnSubEventOccuring.Invoke(this, subEvents[i]);
			}
		}

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
		
		if (!IsReachedEndingDate)
			return false;

		return true;
	}

	void TryStop()
	{
		Debug.Log(eventRef.name + " trying to stop, ending date: " + TimeManager.Date);
		if (OnEventStoping != null)
			OnEventStoping.Invoke(this);
		
		// See if we are ending ending events
		if (TryStopSubEvents())
		{
			// If so, return;
			occuring = false;
			return;
		}

		// Stop the event itself
		for (int i = 0; i < eventRef.modifers.Length; i ++)
		{
			GameDataManager.GameValues.RemoveModifier(eventRef.modifers[i]);
		}

		// Calculate occuring chance of sub events with occuring method of AtTheEnd_One
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

		Debug.Log("ChanceSum: " + chanceSum);
		chanceNum = Random.value * chanceSum;
		chanceSum = 0;

		for (int i = 0; i < subEvents.Length; i++)
		{
			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.Chose_Multiple ||
				subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.Chose_One)
				continue;

			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_One)
			{
				if (chanceSum >= chanceNum ||
					subEvents[i].isExecuting ||
					!subEvents[i].eventRef.StartingCondition.IsSatisfied())
					continue;

				chanceSum += subEvents[i].eventRef.chance;
				if (chanceSum > chanceNum)
				{
					Debug.Log("Subevent " + i + " is executing");
					subEvents[i].Execute();
					if (OnSubEventOccuring != null)
						OnSubEventOccuring.Invoke(this, subEvents[i]);

					chanceNum = float.PositiveInfinity;
				}
			}
			else 
			if (Random.value < subEvents[i].eventRef.chance &&
				subEvents[i].eventRef.StartingCondition.IsSatisfied())
			{
				subEvents[i].Execute();
				if (OnSubEventOccuring != null)
					OnSubEventOccuring.Invoke(this, subEvents[i]);
			}
		}

		foreach (var se in subEvents)
		{
			// Update ending date for event occuring checking
			if (se.occurationEndDate > occurationEndDate)
				occurationEndDate = se.occurationEndDate;
		}

		// If ending date updated, return;
		if (!IsReachedEndingDate) return;

		// Try stopping sub events once more to clean sub events with 0 duration
		TryStopSubEvents();

		Debug.Log(eventRef.name + " stopped, ending date: " + TimeManager.Date);

		occuring = false;
	}

	bool TryStopSubEvents()
	{
		var end = false;
		for (int i = 0; i < subEvents.Length; i++)
		{
			if (!subEvents[i].isExecuting) continue;
			if (subEvents[i].IsReachedEndingDate)
				subEvents[i].Stop();
			if (!subEvents[i].isExecuting && OnSubEventStoping != null)
				OnSubEventStoping.Invoke(this, subEvents[i]);

			if (subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_Multiple ||
				subEvents[i].eventRef.occuringMethod == SubEvent.OccuringMethod.AtTheEnd_One)
				end = true;
		}

		return end;
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

[System.Serializable]
public class RuntimeSubEvent
{
	public readonly SubEvent eventRef;

	public int occurationStartDate = -1;
	public int occurationEndDate = -1;
	public bool isExecuting;

	public bool IsReachedEndingDate
	{
		get { return occurationEndDate <= TimeManager.Date; }
	}

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
		occurationEndDate = eventRef.Duration.RandomBetweenIncluded() + occurationStartDate;
		if (eventRef.Duration != Vector2Int.zero)
			isExecuting = true;

		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			// If this is a event that has no duration(forever), merge modifiers into the basic game values
			if (!isExecuting)
			{
				GameDataManager.GameValues.MergeModifier(eventRef.modifers[i]);
			}
			else
			{
				GameDataManager.GameValues.AddModifier(eventRef.modifers[i]);
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
		if (!isExecuting) return;

		for (int i = 0; i < eventRef.modifers.Length; i++)
		{
			GameDataManager.GameValues.RemoveModifier(eventRef.modifers[i]);
		}
		isExecuting = false;
	}
}
