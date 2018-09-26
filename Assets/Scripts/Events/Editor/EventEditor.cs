using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityUtility;
using UnityUtility.MVS;
using System.Reflection;
using System;

public class EventEditor : EditorWindow
{
	private static Database database;
	private Vector2 scrollPosition;

	private bool confirmDeletion;

	private List<bool> foldOutFlags = new List<bool>();
	private List<bool> secFoldOutFlags = new List<bool>();
	private List<bool> thdFoldOutFlags = new List<bool>();
	private List<bool> forFoldOutFlags = new List<bool>();
	private List<bool> fifFoldOutFlags = new List<bool>();
	private List<bool> sixFoldOutFlags = new List<bool>();
	private List<bool> sevFoldOutFlags = new List<bool>();

	[MenuItem("Event Management/Edit Events")]
	private static void Init()
	{
		GetWindow<EventEditor>();

		database = Database.Instance;
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

		while (secFoldOutFlags.Count < 4)
			secFoldOutFlags.Add(false);


		for (int i = 0; i < list.Count; i++)
		{
			try
			{
				// Displaying one card
				if (i >= foldOutFlags.Count)
					foldOutFlags.Add(false);

				GUILayout.BeginVertical("box");
				foldOutFlags[i] = EditorGUILayout.Foldout(foldOutFlags[i], list[i].name);
				if (foldOutFlags[i])
				{
					EditorUtility.SetDirty(database);

					list[i].ID = i;
					EditorGUILayout.LabelField("ID", i.ToString());

					list[i].name = EditorGUILayout.TextField("Name", list[i].name);

					list[i].scenario = (Scenario)EditorGUILayout.EnumFlagsField("Scenario", list[i].scenario);
					list[i].oddsOfOccuring = EditorGUILayout.FloatField("Odds of Occuring", list[i].oddsOfOccuring);
					list[i].MaxOccurencePerPlaythrough = EditorGUILayout.IntField("Max Occurence", list[i].MaxOccurencePerPlaythrough);
					list[i].Occurence = DrawMinMaxslider(list[i].Occurence, typeof(GameEvent), "Occurence");
					list[i].Duration = DrawMinMaxslider(list[i].Duration, typeof(GameEvent), "Duration");
					list[i].useSubEventMessages = EditorGUILayout.Toggle("Use Sub Event Messages", list[i].useSubEventMessages);

					secFoldOutFlags[0] = EditorGUILayout.Foldout(secFoldOutFlags[0], "Starting Messages");
					if (secFoldOutFlags[0])
					{
						list[i].startingMessage = DrawMessageAreas(list[i].startingMessage);
					}
					secFoldOutFlags[1] = EditorGUILayout.Foldout(secFoldOutFlags[1], "Ending Messages");
					if (secFoldOutFlags[1])
					{
						list[i].endingMessage = DrawMessageAreas(list[i].endingMessage);
					}
					secFoldOutFlags[2] = EditorGUILayout.Foldout(secFoldOutFlags[2], "Modifiers");
					if (secFoldOutFlags[2])
					{
						list[i].modifers = DrawModifiers(list[i].modifers, thdFoldOutFlags);
					}
					secFoldOutFlags[3] = EditorGUILayout.Foldout(secFoldOutFlags[3], "Sub Events");
					if (secFoldOutFlags[3])
					{
						list[i].SubEvents = DrawSubEvents(list[i].SubEvents, forFoldOutFlags, fifFoldOutFlags, sixFoldOutFlags, sevFoldOutFlags);
					}

					// Deletion box
					GUILayout.BeginVertical("box");

					if (!confirmDeletion)
					{
						// Button for Delete this card
						GUI.color = Color.red;
						if (GUILayout.Button("Delete Event"))
						{
							confirmDeletion = true;
						}
						GUI.color = Color.white;
					}
					else
					{
						GUILayout.Label("Are you sure?");
						GUI.color = Color.red;
						if (GUILayout.Button("No"))
						{
							confirmDeletion = false;
						}
						GUI.color = Color.white;

						GUI.color = Color.green;
						if (GUILayout.Button("Yes"))
						{
							list.RemoveAt(i);
							confirmDeletion = false;
							EditorUtility.SetDirty(database);
						}
						GUI.color = Color.white;
					}

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
			list.Add(new GameEvent());
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

	private SubEvent[] DrawSubEvents(SubEvent[] subEvents, List<bool> foldingFlag, List<bool> foldingFlag2, List<bool> foldingFlag3, List<bool> foldingFlag4)
	{
		List<SubEvent> list = new List<SubEvent>(subEvents);
		GUILayout.BeginVertical("box");

		for (int i = 0; i < list.Count; i++)
		{
			if (i >= foldingFlag.Count)
				foldingFlag.Add(false);
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			foldingFlag[i] = EditorGUILayout.Foldout(foldingFlag[i], "Sub Event " + (i + 1));

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
				list[i].occuringMethod = (SubEvent.OccuringMethod)EditorGUILayout.EnumPopup("Occuring Method", list[i].occuringMethod);
				if (list[i].occuringMethod != SubEvent.OccuringMethod.AtTheEnd)
				{
					list[i].chance = EditorGUILayout.FloatField("Occuring Chance", list[i].chance);
				}
				
				list[i].Duration = DrawMinMaxslider(list[i].Duration, typeof(SubEvent), "Duration");
				

				if (i >= foldingFlag2.Count)
					foldingFlag2.Add(false);
				foldingFlag2[i] = EditorGUILayout.Foldout(foldingFlag2[i], "Modifiers");
				if (foldingFlag2[i])
					list[i].modifers = DrawModifiers(list[i].modifers, foldingFlag3);
				
				if ((i + 1) >= foldingFlag4.Count)
				{
					foldingFlag4.Add(false);
				}
				foldingFlag4[i] = EditorGUILayout.Foldout(foldingFlag4[i], "Starting Message");
				if (foldingFlag4[i])
				{
					list[i].startingMessage = DrawMessageAreas(list[i].startingMessage);
				}
				foldingFlag4[i + 1] = EditorGUILayout.Foldout(foldingFlag4[i + 1], "Ending Message");
				if (foldingFlag4[i + 1])
				{
					list[i].endingMessage = DrawMessageAreas(list[i].endingMessage);
				}
			}

			GUILayout.EndVertical();
		}

		GUI.color = Color.green;
		if (GUILayout.Button("+"))
		{
			list.Add(new SubEvent());
		}
		GUI.color = Color.white;
		GUILayout.EndVertical();

		return list.ToArray();
	}

	private GameValueModifer[] DrawModifiers(GameValueModifer[] modifers, List<bool> foldingFlag)
	{
		List<GameValueModifer> list = new List<GameValueModifer>(modifers);
		GUILayout.BeginVertical("box");
		
		for (int i = 0; i < list.Count; i++)
		{
			if (i >= foldingFlag.Count)
				foldingFlag.Add(false);
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			foldingFlag[i] = EditorGUILayout.Foldout(foldingFlag[i], list[i].propertyType.ToString());

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
				list[i].modificationType = (ModificationType)EditorGUILayout.EnumPopup("Modification Type", list[i].modificationType);
				list[i].value_1 = EditorGUILayout.FloatField("Value 1", list[i].value_1);
				list[i].value_2 = EditorGUILayout.FloatField("Value 2", list[i].value_2);
				list[i].value_3 = EditorGUILayout.FloatField("Value 3", list[i].value_3);
			}
			GUILayout.EndVertical();
		}

		GUI.color = Color.green;
		if (GUILayout.Button("+"))
		{
			list.Add(new GameValueModifer());
		}
		GUI.color = Color.white;
		GUILayout.EndVertical();

		return list.ToArray();
	}

	private string[] DrawMessageAreas(string[] messages)
	{
		List<string> list = new List<string>(messages);
		GUILayout.BeginVertical("box");
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

		GUI.color = Color.green;
		if (GUILayout.Button("+"))
		{
			list.Add("");
		}
		GUI.color = Color.white;
		GUILayout.EndVertical();

		return list.ToArray();
	}
}
