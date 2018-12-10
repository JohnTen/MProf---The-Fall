using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityUtility;

public class MinigameManager : EditorWindow
{
	static HarvestingMinigame harvestingGame;

	Vector2 scrollPosition;
	
	bool editingStarvingGame;

	[MenuItem("Minigame/Edit minigames")]
	private static void Init()
	{
		GetWindow<MinigameManager>();
		
		harvestingGame = FindObjectOfType<HarvestingMinigame>();
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal("box");

		if (GUILayout.Button("Harveting Game"))
		{
			editingStarvingGame = true;
		}

		GUILayout.EndHorizontal();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if (editingStarvingGame)
		{
			DrawHarvestingGame();
		}
		
		GUILayout.EndScrollView();

		GUILayout.EndVertical();
	}
	
	#region Haresting

	private void DrawHarvestingGame()
	{
		GUILayout.BeginVertical("box");
		if (!harvestingGame)
		{
			harvestingGame = FindObjectOfType<HarvestingMinigame>();
			if (!harvestingGame)
			{
				GUILayout.Label("You don't have a harvesting game instance in your scene!");
				GUILayout.EndVertical();
				return;
			}
		}
		
		harvestingGame.PointerSpeed = EditorGUILayout.FloatField("Moving speed of pointer", harvestingGame.PointerSpeed);

		GUILayout.EndVertical();
	}

	#endregion

	#region Utilities

	private GameObject CreateNewPrefab(GameObject go, string path)
	{
		var prefab = PrefabUtility.CreateEmptyPrefab(path);
		return PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
	}

	private void IncreaseIndent()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
	}

	private void DecreaseIndent()
	{
		GUILayout.EndHorizontal();
	}

	#endregion
}
