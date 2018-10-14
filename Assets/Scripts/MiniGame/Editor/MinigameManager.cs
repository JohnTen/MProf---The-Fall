using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityUtility;

public class MinigameManager : EditorWindow
{
	static FarmingMinigame farmingGame;
	static StarvingMinigame starvingGame;

	Vector2 scrollPosition;

	bool editingFarmingGame;
	bool editingStarvingGame;

	bool savingAs;
	bool overwriting;
	int saveAsLevel;
	string saveAsName;
	string lastTypeInName;

	MinigameMaze currentEditingMaze;
	GameObject currentEditingMazePrefab;
	int currentEditingLevel = -1;
	int currentEditingLevelIndex = -1;

	const int k_MazeLevelNumber = 4;

	List<bool> foldoutFlags = new List<bool>();

	[MenuItem("Minigame/Edit minigames")]
	private static void Init()
	{
		GetWindow<MinigameManager>();

		farmingGame = FindObjectOfType<FarmingMinigame>();
		starvingGame = FindObjectOfType<StarvingMinigame>();
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal("box");

		if (GUILayout.Button("Farming Game"))
		{
			savingAs = false;
			overwriting = false;
			editingFarmingGame = true;
			editingStarvingGame = false;
		}
		
		if (GUILayout.Button("Starving Game"))
		{
			savingAs = false;
			overwriting = false;
			editingFarmingGame = false;
			editingStarvingGame = true;
		}

		GUILayout.EndHorizontal();

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if (editingFarmingGame)
		{
			DrawFarmingGame();
		}

		if (editingStarvingGame)
		{
			DrawStarvingGame();
		}
		
		GUILayout.EndScrollView();

		GUILayout.EndVertical();
	}

	#region Farming
	private void DrawFarmingGame()
	{
		GUILayout.BeginVertical("box");
		if (!farmingGame)
		{
			farmingGame = FindObjectOfType<FarmingMinigame>();
			if (!farmingGame)
			{
				GUILayout.Label("You don't have a farming game instance in your scene!");
				GUILayout.EndVertical();
				return;
			}
		}

		var mazeList = farmingGame.Level1Mazes;

		while (foldoutFlags.Count < k_MazeLevelNumber)
			foldoutFlags.Add(false);

		// Label for current editing maze
		if (currentEditingMazePrefab != null)
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label("Currently editing " + currentEditingMazePrefab.name);
			GUILayout.EndVertical();
			GUILayout.Space(5);
		}
		
		GUILayout.BeginVertical("box");
		// Button for saving editions
		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Save") &&
			currentEditingMazePrefab != null &&
			currentEditingMaze != null)
		{
			currentEditingMazePrefab = PrefabUtility.ReplacePrefab(
				currentEditingMaze.gameObject,
				currentEditingMazePrefab,
				ReplacePrefabOptions.ConnectToPrefab);

			ReplaceMaze(
				currentEditingLevel,
				currentEditingLevelIndex,
				currentEditingMazePrefab.GetComponent<MinigameMaze>());
		}
		GUI.backgroundColor = Color.white;

		// Button for saving current maze as new prefab
		DrawFarmingSaveAs();

		// Button for close/hide minigame in scene
		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("Close(without save)"))
		{
			currentEditingMaze = null;
			currentEditingMazePrefab = null;
			currentEditingLevel = -1;
			currentEditingLevelIndex = -1;
			farmingGame.Display(false);

			savingAs = false;
			overwriting = false;
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndVertical();

		GUILayout.Space(10);

		GUILayout.BeginVertical("box");
		for (int i = 0; i < k_MazeLevelNumber; i++)
		{
			foldoutFlags[i] = EditorGUILayout.Foldout(foldoutFlags[i], "Level " + (i + 1) + " Mazes");

			if (!foldoutFlags[i]) continue;

			// Select maze list
			switch (i)
			{
				case 0: mazeList = farmingGame.Level1Mazes; break;
				case 1: mazeList = farmingGame.Level2Mazes; break;
				case 2: mazeList = farmingGame.Level3Mazes; break;
				case 3: mazeList = farmingGame.Level4Mazes; break;
			}
			IncreaseIndent();

			GUILayout.BeginVertical();
			for (int m = 0; m < mazeList.Count; m++)
			{
				GUILayout.BeginHorizontal("box");
				mazeList[m] = EditorGUILayout.ObjectField(mazeList[m], typeof(MinigameMaze), false) as MinigameMaze;

				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Edit", GUILayout.Width(50)))
				{
					currentEditingMaze = farmingGame.LoadMaze(i, m);
					currentEditingMazePrefab = mazeList[m].gameObject;
					currentEditingLevel = i;
					currentEditingLevelIndex = m;
					farmingGame.Display(true);

					savingAs = false;
					overwriting = false;
				}
				GUI.backgroundColor = Color.white;

				if (currentEditingLevel == i &&
					currentEditingLevelIndex == m)
				{
					GUILayout.Space(55);
				}
				else
				{
					GUI.backgroundColor = Color.red;
					if (GUILayout.Button("X", GUILayout.Width(50)))
					{
						RemoveMaze(i, m);
					}
					GUI.backgroundColor = Color.white;
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			DecreaseIndent();
		}
		GUILayout.EndVertical();

		GUILayout.EndVertical();
	}

	private bool DrawMazeList(List<MinigameMaze> mazes, bool foldout)
	{


		return foldout;
	}

	private void DrawFarmingSaveAs()
	{
		if (!savingAs)
		{
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Save as") &&
				currentEditingMazePrefab != null &&
				currentEditingMaze != null)
			{
				overwriting = false;
				savingAs = true;
				saveAsName = currentEditingMazePrefab.name;
				lastTypeInName = saveAsName;
			}
			GUI.backgroundColor = Color.white;
			return;
		}

		GUILayout.BeginHorizontal();

		saveAsLevel = EditorGUILayout.IntField(saveAsLevel, GUILayout.Width(30));
		saveAsLevel = Mathf.Clamp(saveAsLevel, 1, k_MazeLevelNumber);
		saveAsName = EditorGUILayout.TextField(saveAsName);

		var path = "Assets/Prefabs/MinigameMazes/Level" + saveAsLevel + "/" + saveAsName + ".prefab";

		if (lastTypeInName != saveAsName)
		{
			overwriting = false;
			lastTypeInName = saveAsName;
		}

		if (overwriting)
		{
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Overwrite?"))
			{
				currentEditingMazePrefab = CreateNewPrefab(currentEditingMaze.gameObject, path);

				ReplaceMaze(
					currentEditingLevel,
					currentEditingLevelIndex,
					currentEditingMazePrefab.GetComponent<MinigameMaze>());

				overwriting = false;
				savingAs = false;
			}
			GUI.backgroundColor = Color.white;
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("Cancel"))
			{
				overwriting = false;
			}
			GUI.backgroundColor = Color.white;
		}
		else
		{
			GUI.backgroundColor = Color.green;
			if (GUILayout.Button("Save"))
			{
				if (AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)))
				{
					overwriting = true;
				}
				else
				{
					currentEditingMazePrefab = CreateNewPrefab(currentEditingMaze.gameObject, path);

					currentEditingLevel = saveAsLevel - 1;
					currentEditingLevelIndex = AddMaze(
						currentEditingLevel,
						currentEditingMazePrefab.GetComponent<MinigameMaze>());

					savingAs = false;
				}
			}
		}
		GUI.backgroundColor = Color.white;

		GUILayout.EndHorizontal();
	}

	private int AddMaze(int level, MinigameMaze maze)
	{
		List<MinigameMaze> list = new List<MinigameMaze>();
		switch (level)
		{
			case 0: list = farmingGame.Level1Mazes; break;
			case 1: list = farmingGame.Level2Mazes; break;
			case 2: list = farmingGame.Level3Mazes; break;
			case 3: list = farmingGame.Level4Mazes; break;
		}

		list.Add(maze);
		return list.Count - 1;
	}

	private void ReplaceMaze(int level, int index, MinigameMaze maze)
	{
		switch (level)
		{
			case 0: farmingGame.Level1Mazes[index] = maze; break;
			case 1: farmingGame.Level2Mazes[index] = maze; break;
			case 2: farmingGame.Level3Mazes[index] = maze; break;
			case 3: farmingGame.Level4Mazes[index] = maze; break;
		}
	}

	private void RemoveMaze(int level, int index)
	{
		GameObject prefab = null;
		List<MinigameMaze> list = new List<MinigameMaze>();

		switch (level)
		{
			case 0: list = farmingGame.Level1Mazes; break;
			case 1: list = farmingGame.Level2Mazes; break;
			case 2: list = farmingGame.Level3Mazes; break;
			case 3: list = farmingGame.Level4Mazes; break;
		}

		prefab = PrefabUtility.FindPrefabRoot(list[index].gameObject);
		AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
		list.RemoveAt(index);
	}
	#endregion

	#region Starving

	private void DrawStarvingGame()
	{
		GUILayout.BeginVertical("box");
		if (!starvingGame)
		{
			starvingGame = FindObjectOfType<StarvingMinigame>();
			if (!starvingGame)
			{
				GUILayout.Label("You don't have a starving game instance in your scene!");
				GUILayout.EndVertical();
				return;
			}
		}

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
