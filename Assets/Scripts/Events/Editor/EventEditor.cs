using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityUtility;
using UnityUtility.MVS;

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
				while (i * 7 + 6 >= foldOutFlags.Count)
					foldOutFlags.Add(false);
				while (i >= deleteFlags.Count)
					deleteFlags.Add(false);

				GUILayout.BeginVertical("box");
				GUILayout.BeginHorizontal();
				foldOutFlags[i * 7] = EditorGUILayout.Foldout(foldOutFlags[i * 7], list[i].name);

				if (!deleteFlags[i])
				{
					GUI.color = Color.red;
					if (GUILayout.Button("Delete", GUILayout.Width(60)))
					{
						deleteFlags[i] = true;
					}
					GUI.color = Color.white;
				}
				else
				{
					GUI.color = Color.green;
					if (GUILayout.Button("Yes", GUILayout.Width(50)))
					{
						list.RemoveAt(i);
						deleteFlags.RemoveAt(i);
						EditorUtility.SetDirty(database);
						i--;
					}
					GUI.color = Color.white;

					GUI.color = Color.red;
					if (GUILayout.Button("No", GUILayout.Width(50)))
					{
						deleteFlags[i] = false;
					}
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();

				if (foldOutFlags[i * 7])
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

					foldOutFlags[i * 7 + 1] = DrawConditionGroup("Starting Condition"	, foldOutFlags[i * 7 + 1], ref list[i].StartingCondition);
					foldOutFlags[i * 7 + 2] = DrawConditionGroup("Continual Condition"	, foldOutFlags[i * 7 + 2], ref list[i].ContinualCondition);
					foldOutFlags[i * 7 + 3] = DrawMessageAreas	("Starting Message"		, foldOutFlags[i * 7 + 3], ref list[i].startingMessage);
					foldOutFlags[i * 7 + 4] = DrawMessageAreas	("Ending Message"		, foldOutFlags[i * 7 + 4], ref list[i].endingMessage);
					foldOutFlags[i * 7 + 5] = DrawModifiers		("Modifiers"			, foldOutFlags[i * 7 + 5], secFoldOutFlags, ref list[i].modifers);
					foldOutFlags[i * 7 + 6] = DrawSubEvents		("Sub Events"			, foldOutFlags[i * 7 + 6], thdFoldOutFlags, forFoldOutFlags, fifFoldOutFlags, ref list[i].subEvents);
					
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

		GUI.color = Color.green;
		if (GUILayout.Button("+"))
		{
			list.Add(new GameEvent(list[list.Count - 1]));
		}
		GUI.color = Color.white;

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

		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(new SubEvent(list[list.Count - 1]));
			else
				list.Add(new SubEvent());
		}
		GUI.color = Color.white;
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

			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			if (foldingFlag[i])
			{
				list[i].label = EditorGUILayout.TextField("Label", list[i].label);
				list[i].occuringMethod = (SubEvent.OccuringMethod)EditorGUILayout.EnumPopup("Occuring Method", list[i].occuringMethod);
				DynamicValueField("Occuring Chance", list[i].chance);
				
				list[i].Duration = DrawMinMaxslider(list[i].Duration, typeof(SubEvent), "Duration");
				
				while ((i * 5 + 4) >= foldingFlag2.Count)
					foldingFlag2.Add(false);
				
				foldOutFlags[i * 5 + 0] = DrawConditionGroup("Starting Condition"	, foldOutFlags[i * 5 + 0], ref list[i].StartingCondition);
				foldOutFlags[i * 5 + 1] = DrawConditionGroup("Continual Condition"	, foldOutFlags[i * 5 + 1], ref list[i].ContinualCondition);
				foldingFlag2[i * 5 + 2] = DrawMessageAreas	("Starting Message"		, foldingFlag2[i * 5 + 2], ref list[i].startingMessage);
				foldingFlag2[i * 5 + 3] = DrawMessageAreas	("Ending Message"		, foldingFlag2[i * 5 + 3], ref list[i].endingMessage);
				foldingFlag2[i * 5 + 4] = DrawModifiers		("Modifiers"			, foldingFlag2[i * 5 + 4], foldingFlag3, ref list[i].modifers);
			}

			GUILayout.EndVertical();
		}
		
		GUILayout.EndVertical();
		MinusIndent();
		GUILayout.EndVertical();

		subEvents = list.ToArray();
		
		return foldout;
	}

	private bool DrawConditionGroup(string label, bool foldout, ref EventConditionGroup group)
	{
		var list = group.conditions;
		var notList = new List<bool>();
		var oprList = new List<LogicalOperator>();

		GUILayout.BeginVertical("box");
		GUILayout.BeginHorizontal();
		foldout = EditorGUILayout.Foldout(foldout, label);

		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
			{
				group.operators.Add(LogicalOperator.And);
				list.Add(new EventCondition(list[list.Count - 1]));
			}
			else
				list.Add(new EventCondition());
		}
		GUI.color = Color.white;
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
			GUI.color = Color.red;
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
			GUI.color = Color.white;
			GUILayout.Space(3);

			// Toggle for switch between EventCondition and ValueCondition
			var isEventCond = list[i].type == ConditionType.GameEvent;
			isEventCond = EditorGUILayout.Toggle(isEventCond);
			if ((list[i].type == ConditionType.GameEvent) != isEventCond)
			{
				list[i] = new EventCondition()
				{
					type = isEventCond ? ConditionType.GameEvent : ConditionType.GameValue
				};
			}

			// Button for toggle Not
			if (notList[i])
				GUI.color = Color.green;
			else
				GUI.color = Color.red;
			if (GUILayout.Button("Not", GUILayout.Width(45)))
			{
				notList[i] = !notList[i];
			}
			GUI.color = Color.white;

			if (list[i].type == ConditionType.GameValue)
				ValueConditionField(list[i]);
			else
				EventConditionField(list[i]);

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

		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(new GameValueModifer(list[list.Count - 1]));
			else
				list.Add(new GameValueModifer());
		}
		GUI.color = Color.white;
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

			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.color = Color.white;
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

		GUI.color = Color.green;
		if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
		{
			if (list.Count > 0)
				list.Add(list[list.Count - 1]);
			else
				list.Add("");
		}
		GUI.color = Color.white;
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

			GUI.color = Color.red;
			if (GUILayout.Button("X", GUILayout.MaxWidth(50)))
			{
				list.RemoveAt(i);
				continue;
			}
			GUI.color = Color.white;

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
