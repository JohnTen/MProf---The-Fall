using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using UnityUtility;

[System.Serializable]
public class GameData
{
	public int Date;
	public GameValues gameValues;
	public List<FieldBlockStatus> fieldBlocks;
}

public class GameDataManager : GlobalSingleton<GameDataManager>
{
	[SerializeField] Camera mainCamera;
	[SerializeField] GameValues gameValues;

	public static event Action OnValueChanged;

	public static Camera MainCamera
	{
		get
		{
			if (Instance.mainCamera == null)
				Instance.mainCamera = Camera.main;

			return Instance.mainCamera;
		}
	}

	//public static GameValuesOld Values
	//{
	//	get { return Instance.values; }
	//}

	//public static GameValuesOld EventOffset
	//{
	//	get { return Instance.offset; }
	//}

	public static GameValues GameValues
	{
		get { return Instance.gameValues; }
	}

	public static int CurrentWheat
	{
		get { return (int)GameValues[GameValueType.NumberOfWheat]; }
		set
		{
			GameValues[GameValueType.NumberOfWheat] = value;
			UpdateValues();
		}
	}
	
	public static int CurrentOat
	{
		get { return (int)GameValues[GameValueType.NumberOfOat]; }
		set
		{
			GameValues[GameValueType.NumberOfOat] = value;
			UpdateValues();
		}
	}

	public static int CurrentWheatSeed
	{
		get { return (int)GameValues[GameValueType.NumberOfWheatSeed]; }
		set
		{
			GameValues[GameValueType.NumberOfWheatSeed] = value;
			UpdateValues();
		}
	}

	public static int CurrentOatSeed
	{
		get { return (int)GameValues[GameValueType.NumberOfOatSeed]; }
		set
		{
			GameValues[GameValueType.NumberOfOatSeed] = value;
			UpdateValues();
		}
	}

	public static int CurrentFertiliser
	{
		get { return (int)GameValues[GameValueType.NumberOfFertiliser]; }
		set
		{
			GameValues[GameValueType.NumberOfFertiliser] = value;
			UpdateValues();
		}
	}

	public static int CurrentWife
	{
		get { return (int)GameValues[GameValueType.NumberOfWife]; }
		set
		{
			GameValues[GameValueType.NumberOfWife] = value;
			UpdateValues();
		}
	}

	public static int CurrentSon
	{
		get { return (int)GameValues[GameValueType.NumberOfSon]; }
		set
		{
			GameValues[GameValueType.NumberOfSon] = value;
			UpdateValues();
		}
	}

	public static int CurrentDaughter
	{
		get { return (int)GameValues[GameValueType.NumberOfDaughter]; }
		set
		{
			GameValues[GameValueType.NumberOfDaughter] = value;
			UpdateValues();
		}
	}

	public static int CurrentChicken
	{
		get { return (int)GameValues[GameValueType.NumberOfChicken]; }
		set
		{
			GameValues[GameValueType.NumberOfChicken] = value;
			UpdateValues();
		}
	}

	public static int CurrentOx
	{
		get { return (int)GameValues[GameValueType.NumberOfOx]; }
		set
		{
			GameValues[GameValueType.NumberOfOx] = value;
			UpdateValues();
		}
	}

	public static int CurrentFamilyHunger
	{
		get { return (int)GameValues[GameValueType.FamilyHunger]; }
		set
		{
			GameValues[GameValueType.FamilyHunger] = value;
			UpdateValues();
		}
	}

	public static int CurrentAnimalHunger
	{
		get { return (int)GameValues[GameValueType.AnimalHunger]; }
		set
		{
			GameValues[GameValueType.AnimalHunger] = value;
			UpdateValues();
		}
	}

	public static int GetCropNumber(int id)
	{
		switch (id)
		{
			case 0: return (int)GameValues[GameValueType.NumberOfWheat];
			case 1: return (int)GameValues[GameValueType.NumberOfOat];
		}

		return -1;
	}

	public static int GetCropSeedNumbr(int id)
	{
		switch (id)
		{
			case 0: return (int)GameValues[GameValueType.NumberOfWheatSeed];
			case 1: return (int)GameValues[GameValueType.NumberOfOatSeed];
		}

		return -1;
	}

	public static int GetAnimalNumber(int id)
	{
		switch (id)
		{
			case 0: return (int)GameValues[GameValueType.NumberOfChicken];
			case 1: return (int)GameValues[GameValueType.NumberOfOx];
		}

		return -1;
	}

	public static int GetFamilyNumber(int id)
	{
		switch (id)
		{
			case 0: return (int)GameValues[GameValueType.NumberOfWife];
			case 1: return (int)GameValues[GameValueType.NumberOfSon];
		}

		return -1;
	}

	public static void ModifyCropNumber(int id, int value)
	{
		switch (id)
		{
			case 0: GameValues[GameValueType.NumberOfWheat] += value; break;
			case 1: GameValues[GameValueType.NumberOfOat] += value; break;
		}
	}

	public static void ModifyCropSeedNumbr(int id, int value)
	{
		switch (id)
		{
			case 0: GameValues[GameValueType.NumberOfWheatSeed] += value; break;
			case 1: GameValues[GameValueType.NumberOfOatSeed] += value; break;
		}
	}

	public static void ModifyAnimalNumber(int id, int value)
	{
		switch (id)
		{
			case 0: GameValues[GameValueType.NumberOfChicken] += value; break;
			case 1: GameValues[GameValueType.NumberOfOx] += value; break;
		}
	}

	public static void ModifyFamilyNumber(int id, int value)
	{
		switch (id)
		{
			case 0: GameValues[GameValueType.NumberOfWife] += value; break;
			case 1: GameValues[GameValueType.NumberOfSon] += value; break;
		}
	}

	public static void UpdateValues()
	{
		if (OnValueChanged != null)
			OnValueChanged.Invoke();
	}

	public static void SaveGame()
	{
		Instance._Save();
	}

	public void _Save()
	{
		GameData data = new GameData();

		data.Date = TimeManager.Date;
		data.gameValues = gameValues;
		data.fieldBlocks = new List<FieldBlockStatus>();

		for (int i = 0; i < FieldManager.Instance.FieldBlocks.Count; i++)
		{
			data.fieldBlocks.Add(FieldManager.Instance.FieldBlocks[i].Status);
		}

		FileStream fs = new FileStream(Application.persistentDataPath + "/game.sav", FileMode.Create);
		BinaryFormatter bf = new BinaryFormatter();

		bf.Serialize(fs, data);
		fs.Close();
	}

	public static void Load()
	{
		Instance._Load();
	}

	public void _Load()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs;

		try
		{
			fs = File.Open(Application.persistentDataPath + "/game.sav", FileMode.Open);
		}
		catch (FileNotFoundException)
		{
			Debug.Log("Save file is not found.");
			return;
		}

		object deserialized = null;
		GameData game;
		try
		{
			deserialized = bf.Deserialize(fs);
			game = (GameData)deserialized;
		}
		catch (SerializationException)
		{
			Debug.LogWarning("Loading game save failed");
			fs.Close();
			return;
		}

		fs.Close();
		SettingGame(game);
	}

	void SettingGame(GameData data)
	{
		TimeManager.Date = data.Date;
		gameValues = data.gameValues;
		UpdateValues();

		var field = FieldManager.Instance.FieldBlocks;
		if (field.Count != data.fieldBlocks.Count)
		{
			Debug.LogWarning("Data doesn't match the actual field!");
		}

		for (int i = 0; i < field.Count; i ++)
		{
			field[i].Status = data.fieldBlocks[i];
		}
	}
}
