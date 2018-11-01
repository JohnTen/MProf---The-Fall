using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityUtility;
using UnityUtility.MVS;
using UnityUtility.Conditions;

public class EventEditor : EditorWindow
{
	private static GameDatabase database;
	private Vector2 scrollPosition;

	private List<bool> deleteFlags = new List<bool>();
	private List<bool> foldOutFlags = new List<bool>();
	private List<bool> secFoldOutFlags = new List<bool>();
	private List<bool> thdFoldOutFlags = new List<bool>();
	private List<bool> forFoldOutFlags = new List<bool>();
	private List<bool> fifFoldOutFlags = new List<bool>();

	[MenuItem("Event Management/Edit Events")]
	private static void Init()
	{
		GetWindow<EventEditor>();

		database = GameDatabase.Instance;
	}

	private void OnGUI()
	{
		if (database == null)
		{
			Init();
		}

		GUILayout.BeginVertical();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		GUILayout.Space(10);

		var list = database.eventList;

		for (int i = 0; i < list.Count; i++)
		{
			try
			{
				while (i * 8 + 7 >= foldOutFlags.Count)
					foldOutFlags.Add(false);
				while (i >= deleteFlags.Count)
					deleteFlags.Add(false);

				GUILayout.BeginVertical("box");
				GUILayout.BeginHorizontal();
				foldOutFlags[i * 8] = EditorGUILayout.Foldout(foldOutFlags[i * 8], list[i].name);

				if (!deleteFlags[i])
				{
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("Delete", GUILayout.Width(60)))
					{
						deleteFlags[i] = true;
					}
					GUI.backgroundColor = Color.white;
				}
				else
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button("Yes", GUILayout.Width(50)))
					{
						list.RemoveAt(i);
						deleteFlags.RemoveAt(i);
						EditorUtility.SetDirty(database);
						i--;
					}
					GUI.backgroundColor = Color.white;

					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("No", GUILayout.Width(50)))
					{
						deleteFlags[i] = false;
					}
					GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal();

				if (foldOutFlags[i * 8])
				{
					AddIndent();
					GUILayout.BeginVertical();
					EditorUtility.SetDirty(database);

					list[i].ID = i;
					EditorGUILayout.LabelField("ID", i.ToString());

					list[i].name = EditorGUILayout.TextField("Name", list[i].name);

					list[i].scenario = (Scenario)EditorGUILayout.EnumFlagsField("Scenario", list[i].scenario);
					DynamicValueField("Odds of Occuring", list[i].oddsOfOccuring);
					DynamicValueField("Max Occurence", list[i].MaxOccurencePerPlaythrough);
					list[i].Occurence = DrawMinMaxslider(list[i].Occurence, typeof(GameEvent), "Occurence");
					list[i].Duration = DrawMinMaxslider(list[i].Duration, typeof(GameEvent), "Duration");
					list[i].useSubEventMessages = EditorGUILayout.Toggle("Use Sub Event Messages", list[i].useSubEventMessages);

					foldOutFlags[i * 8 + 1] = DrawConditionGroup("Starting Condition"	, foldOutFlags[i * 8 + 1], list[i].StartingCondition);
					foldOutFlags[i * 8 + 2] = DrawConditionGroup("Continual Condition"	, foldOutFlags[i * 8 + 2], list[i].ContinualCondition);
					foldOutFlags[i * 8 + 3] = DrawConditionGroup("Family Condition"		, foldOutFlags[i * 8 + 3], list[i].FamilyCondition);
					foldOutFlags[i * 8 + 4] = DrawMessageAreas	("Starting Message"		, foldOutFlags[i * 8 + 4], ref list[i].startingMessage);
					foldOutFlags[i * 8 + 5] = DrawMessageAreas	("Ending Message"		, foldOutFlags[i * 8 + 5], ref list[i].endingMessage);
					foldOutFlags[i * 8 + 6] = DrawModifiers		("Modifiers"			, foldOutFlags[i * 8 + 6], secFoldOutFlags, ref list[i].modifers);
					foldOutFlags[i * 8 + 7] = DrawSubEvents		("Sub Events"			, foldOutFlags[i * 8 + 7], thdFoldOutFlags, forFoldOutFlags, fifFoldOutFlags, ref list[i].subEvents);
					
					MinusIndent();
					GUILayout.EndVertical();
				}
				else
				{
					list[i].ID = i;
				}

				GUILayout.EndVertical();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
		}

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("+"))
		{
			list.Add(new GameEvent(list[list.Count - 1]));
		}
		GUI.backgroundColor = Color.white;
		
		if (GUILayout.Button("Check list"))
		{
			for (int i = 0; i < foldOutFlags.Count; i++)
			{
				foldOutFlags[i] = true;
			}
			for (int i = 0; i < secFoldOutFlags.Count; i ++)
			{
				secFoldOutFlags[i] = true;
			}
			for (int i = 0; i < thdFoldOutFlags.Count; i++)
			{
				thdFoldOutFlags[i] = true;
			}
			for (int i = 0; i < forFoldOutFlags.Count; i++)
			{
				forFoldOutFlags[i] = true;
			}
			for (int i = 0; i < fifFoldOutFlags.Count; i++)
			{
				fifFoldOutFlags[i] = true;
			}
		}

		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	private Vector2Int DrawMinMaxslider(Vector2 vector, Type type, string name)
	{
		GUILayout.BeginHorizontal();
		float min = vector.x;
		float max = vector.y;
		var attr = (MinMaxSlider)Attribute.GetCustomAttribute(type.GetField(name), typeof(MinMaxSlider));
		float minLim = attr.min;
		float maxLim = attr.max;
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField(name, GUILayout.Width(150));
		EditorGUILayout.LabelField(min.ToString(), GUILayout.Width(20));
		EditorGUILayout.MinMaxSlider(ref min, ref max, minLim, maxLim, GUILayout.ExpandWidth(true));
		EditorGUILayout.LabelField(max.ToString(), GUILayout.Width(20));
		GUILayout.EndHorizontal();
		return new Vector2Int(Mathf.RoundToInt(min), Mathf.RoundToInt(max));
	}

	private bool DrawSubEvents(string label, bool foldout, List<bool> foldingFlag, List<bool> foldingFlag2, List<bool> foldingFlag3, ref SubEvent[] subEvents)
	{
		List<SubEvent> list = new List<SubEvent>(subEvents);

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		foldout = EditorGUILayout.Foldout(foldout, label);

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(new SubEvent(list[list.Count - 1]));
			else
				list.Add(new SubEvent());
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();

		if (!foldout)
		{
			GUILayout.EndVertical();
			return foldout;
		}

		AddIndent();
		GUILayout.BeginVertical();

		for (int i = 0; i < list.Count; i++)
		{
			if (i >= foldingFlag.Count)
				foldingFlag.Add(false);
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			foldingFlag[i] = EditorGUILayout.Foldout(foldingFlag[i], list[i].label);

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			if (foldingFlag[i])
			{
				list[i].label = EditorGUILayout.TextField("Label", list[i].label);
				list[i].occuringMethod = (SubEvent.OccuringMethod)EditorGUILayout.EnumPopup("Occuring Method", list[i].occuringMethod);
				DynamicValueField("Occuring Chance", list[i].chance);
				
				list[i].Duration = DrawMinMaxslider(list[i].Duration, typeof(SubEvent), "Duration");
				
				while ((i * 6 + 5) >= foldingFlag2.Count)
					foldingFlag2.Add(false);

				foldingFlag2[i * 6 + 0] = DrawConditionGroup("Starting Condition"	, foldingFlag2[i * 6 + 0], list[i].StartingCondition);
				foldingFlag2[i * 6 + 1] = DrawConditionGroup("Continual Condition"	, foldingFlag2[i * 6 + 1], list[i].ContinualCondition);
				foldingFlag2[i * 6 + 2] = DrawConditionGroup("Family Condition"		, foldingFlag2[i * 6 + 2], list[i].FamilyCondition);
				foldingFlag2[i * 6 + 3] = DrawMessageAreas	("Starting Message"		, foldingFlag2[i * 6 + 3], ref list[i].startingMessage);
				foldingFlag2[i * 6 + 4] = DrawMessageAreas	("Ending Message"		, foldingFlag2[i * 6 + 4], ref list[i].endingMessage);
				foldingFlag2[i * 6 + 5] = DrawModifiers		("Modifiers"			, foldingFlag2[i * 6 + 5], foldingFlag3, ref list[i].modifers);
			}

			GUILayout.EndVertical();
		}
		
		GUILayout.EndVertical();
		MinusIndent();
		GUILayout.EndVertical();

		subEvents = list.ToArray();
		
		return foldout;
	}

	private bool DrawConditionGroup<T>(string label, bool foldout, BaseConditionGroup<T> group) where T : class, IConditionable, new()
	{
		var list = group.conditions;
		var notList = new List<bool>();
		var oprList = new List<LogicalOperator>();

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		foldout = EditorGUILayout.Foldout(foldout, label);

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
			{
				group.operators.Add(LogicalOperator.And);
				list.Add((T)list[list.Count - 1].Clone());
			}
			else
				list.Add(new T());
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();

		if (!foldout)
		{
			GUILayout.EndVertical();
			return foldout;
		}

		// Seperate Not from other two operators
		for (int i = 0; i < list.Count; i++)
		{
			notList.Add(false);
		}

		for (int i = 0, j = 0; i < group.operators.Count; i++)
		{
			if (group.operators[i] == LogicalOperator.Not)
			{
				notList[j] = true;
			}
			else
			{
				oprList.Add(group.operators[i]);
				j++;
				if (j >= list.Count)
					break;
			}
		}

		AddIndent();
		GUILayout.BeginVertical();

		for (int i = 0; i < list.Count; i ++)
		{
			GUILayout.BeginHorizontal("box");

			// Button for delete this condition
			GUI.backgroundColor = Color.red;
			if(GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				notList.RemoveAt(i);
				if (oprList.Count > i + 1)
					oprList.RemoveAt(i);
				else if (i - 1 >= 0)
					oprList.RemoveAt(i - 1);

				continue;
			}
			GUI.backgroundColor = Color.white;
			GUILayout.Space(3);

			// Draw event condition
			if ((list[i] as EventCondition) != null)
			{
				// Toggle for switch between EventCondition and ValueCondition
				var e = list[i] as EventCondition;
				var isEventCond = e.type == ConditionType.GameEvent;
				isEventCond = EditorGUILayout.Toggle(isEventCond);
				if ((e.type == ConditionType.GameEvent) != isEventCond)
				{
					e = new EventCondition()
					{
						type = isEventCond ? ConditionType.GameEvent : ConditionType.GameValue
					};
					list[i] = e as T;
				}
			}

			// Button for toggle Not
			if (notList[i])
				GUI.backgroundColor = Color.green;
			else
				GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Not", GUILayout.Width(45)))
			{
				notList[i] = !notList[i];
			}
			GUI.backgroundColor = Color.white;

			// Draw event condition
			if ((list[i] as EventCondition) != null)
			{
				var e = list[i] as EventCondition;
				if (e.type == ConditionType.GameValue)
					ValueConditionField(e);
				else
					EventConditionField(e);
			}

			// Draw family condition
			if ((list[i] as FamilyCondition) != null)
			{
				var e = list[i] as FamilyCondition;
				FamilyConditionField(e);
			}

			// Logical operators between conditions, the last condition won't need one
			if (i < oprList.Count)
			{
				if (GUILayout.Button(oprList[i].ToString(), GUILayout.Width(45)))
				{
					if (oprList[i] == LogicalOperator.And)
						oprList[i] = LogicalOperator.Or;
					else
						oprList[i] = LogicalOperator.And;
				}
			}
			else
			{
				GUILayout.Space(50);
			}
			GUILayout.EndHorizontal();

		}

		// Update operators
		group.operators.Clear();
		for (int i = 0; i < notList.Count; i++)
		{
			if (notList[i])
				group.operators.Add(LogicalOperator.Not);
			if (oprList.Count > i)
				group.operators.Add(oprList[i]);
		}

		GUILayout.EndVertical();
		MinusIndent();
		GUILayout.EndVertical();

		return foldout;
	}

	private void FamilyConditionField(FamilyCondition condition)
	{
		condition.type = (FamilyValueType)EditorGUILayout.EnumPopup(condition.type);
		condition.@operator = (RelationalOperator)EditorGUILayout.EnumPopup(condition.@operator);
		switch (condition.type)
		{
			case FamilyValueType.MemberType:
				condition.value = (int)(FamilyType)EditorGUILayout.EnumPopup((FamilyType)((int)condition.value));
				break;
			case FamilyValueType.Hunger:
				condition.value = EditorGUILayout.IntField((int)condition.value);
				break;
			case FamilyValueType.DyingRate:
			case FamilyValueType.MentalHealth:
				condition.value = EditorGUILayout.FloatField(condition.value);
				break;
		}
	}

	private void ValueConditionField(EventCondition condition)
	{
		DynamicValueField(condition.value_1);
		condition.@operator = (RelationalOperator)EditorGUILayout.EnumPopup(condition.@operator);
		DynamicValueField(condition.value_2);
	}

	private void EventConditionField(EventCondition condition)
	{
		GUILayout.Label("When event");
		condition.value_1.constantValue = EditorGUILayout.IntField((int)condition.value_1.constantValue);
		GUILayout.Label(" is running.");
	}

	private bool DrawModifiers(string label, bool foldout, List<bool> foldingFlag, ref GameValueModifer[] modifers)
	{
		List<GameValueModifer> list = new List<GameValueModifer>(modifers);

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		foldout = EditorGUILayout.Foldout(foldout, label);

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(new GameValueModifer(list[list.Count - 1]));
			else
				list.Add(new GameValueModifer());
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();

		if (!foldout)
		{
			GUILayout.EndVertical();
			return foldout;
		}

		AddIndent();
		GUILayout.BeginVertical();
		
		for (int i = 0; i < list.Count; i++)
		{
			if (i >= foldingFlag.Count)
				foldingFlag.Add(false);

			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();

			DeserialiseGameValueType(list[i]);
			foldingFlag[i] = EditorGUILayout.Foldout(foldingFlag[i], list[i].serializedtype);

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();
			if (foldingFlag[i])
			{
				list[i].propertyType = (GameValueType)EditorGUILayout.EnumPopup("Property Type", list[i].propertyType);
				list[i].serializedtype = list[i].propertyType.ToString();
				list[i].modificationType = (ModificationType)EditorGUILayout.EnumPopup("Modification Type", list[i].modificationType);
				list[i].value_1 = EditorGUILayout.FloatField("Value 1", list[i].value_1);
				list[i].value_2 = EditorGUILayout.FloatField("Value 2", list[i].value_2);
				list[i].value_3 = EditorGUILayout.FloatField("Value 3", list[i].value_3);
			}
			GUILayout.EndVertical();
		}

		GUILayout.EndVertical();
		MinusIndent();
		GUILayout.EndVertical();

		modifers = list.ToArray();
		return foldout;
	}

	private bool DrawMessageAreas(string label, bool foldout, ref string[] messages)
	{
		List<string> list = new List<string>(messages);

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		foldout = EditorGUILayout.Foldout(foldout, label);

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(list[list.Count - 1]);
			else
				list.Add("");
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();

		if (!foldout)
		{
			GUILayout.EndVertical();
			return foldout;
		}

		AddIndent();
		GUILayout.BeginVertical();
		for (int i = 0; i < list.Count; i ++)
		{
			GUILayout.BeginHorizontal("box");

			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.backgroundColor = Color.white;

			list[i] = EditorGUILayout.TextArea(list[i]);
			GUILayout.EndHorizontal();
		}

		messages = list.ToArray();
		
		GUILayout.EndVertical();
		GUILayout.EndVertical();

		MinusIndent();
		return foldout;
	}

	private void DynamicValueField(DynamicValue dv)
	{
		GUILayout.BeginHorizontal();
		dv.useGameValue = EditorGUILayout.Toggle(dv.useGameValue, GUILayout.Width(20));

		DeserialiseGameValueType(dv);
		if (dv.useGameValue)
		{
			dv.valueType = (GameValueType)EditorGUILayout.EnumPopup(dv.valueType);
			dv.serializedtype = dv.valueType.ToString();
		}
		else
		{
			dv.constantValue = EditorGUILayout.FloatField(dv.constantValue);
		}
		GUILayout.EndHorizontal();
	}

	private void DynamicValueField(string label, DynamicValue dv)
	{
		GUILayout.BeginHorizontal();
		dv.useGameValue = EditorGUILayout.Toggle(label, dv.useGameValue, GUILayout.ExpandWidth(false));

		DeserialiseGameValueType(dv);
		if (dv.useGameValue)
		{
			dv.valueType = (GameValueType)EditorGUILayout.EnumPopup(dv.valueType);
			dv.serializedtype = dv.valueType.ToString();
		}
		else
		{
			dv.constantValue = EditorGUILayout.FloatField(dv.constantValue);
		}
		GUILayout.EndHorizontal();
	}

	private void DeserialiseGameValueType(GameValueModifer modifer)
	{
		GameValueType type;
		try
		{
			type = (GameValueType)Enum.Parse(typeof(GameValueType), modifer.serializedtype);
		}
		catch
		{
			type = GameValueType.CropProduction;
		}
		modifer.propertyType = type;
		modifer.serializedtype = type.ToString();
	}

	private void DeserialiseGameValueType(DynamicValue value)
	{
		GameValueType type;
		try
		{
			type = (GameValueType)Enum.Parse(typeof(GameValueType), value.serializedtype);
		}
		catch
		{
			type = GameValueType.CropProduction;
		}
		value.valueType = type;
		value.serializedtype = type.ToString();
	}

	public void AddIndent()
	{
		//EditorGUI.indentLevel++;
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
	}

	public void MinusIndent()
	{
		//EditorGUI.indentLevel--;
		GUILayout.EndHorizontal();
	}
}
