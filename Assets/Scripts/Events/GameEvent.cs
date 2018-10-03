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
	public EventConditionGroup StartingCondition = new EventConditionGroup();
	public EventConditionGroup ContinualCondition = new EventConditionGroup();

	public DynamicValue MaxOccurencePerPlaythrough = new DynamicValue();
	[MinMaxSlider(0, 30)] public Vector2Int Occurence;
	[MinMaxSlider(0, 10)] public Vector2Int Duration;

	public GameValueModifer[] modifers = new GameValueModifer[0];
	public SubEvent[] subEvents = new SubEvent[0];

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

		subEvents = new SubEvent[gEvent.subEvents.Length];
		for (int i = 0; i < subEvents.Length; i++)
		{
			subEvents[i] = new SubEvent(gEvent.subEvents[i]);
		}
	}
}

[System.Serializable]
public class SubEvent
{
	public enum OccuringMethod
	{
		Chose_One,
		Chose_Multiple,
		AtTheEnd_One,
		AtTheEnd_Multiple,
	}

	public string label = "Sub event";
	public OccuringMethod occuringMethod;
	public DynamicValue chance = new DynamicValue();
	[MinMaxSlider(0, 20)] public Vector2Int Duration;
	public GameValueModifer[] modifers = new GameValueModifer[0];
	public string[] startingMessage = new string[0];
	public string[] endingMessage = new string[0];

	public EventConditionGroup StartingCondition = new EventConditionGroup();
	public EventConditionGroup ContinualCondition = new EventConditionGroup();

	public SubEvent() { }

	public SubEvent(SubEvent sEvent)
	{
		label = sEvent.label;
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
}
