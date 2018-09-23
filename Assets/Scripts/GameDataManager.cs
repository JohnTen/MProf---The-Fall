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
	public GameValues values;
	public GameValues offset;
	public List<FieldBlockStatus> fieldBlocks;
}

public class GameDataManager : GlobalSingleton<GameDataManager>
{
	[SerializeField] Camera mainCamera;
	[SerializeField] GameValues values;
	[SerializeField] GameValues offset;

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

	public static GameValues Values
	{
		get { return Instance.values; }
	}

	public static GameValues EventOffset
	{
		get { return Instance.offset; }
	}

	public static int[] CurrentCrops
	{
		get { return Instance.values.crops; }
	}

	public static int[] CurrentCropSeeds
	{
		get { return Instance.values.cropSeeds; }
	}

	public static int[] CurrentAnimals
	{
		get { return Instance.values.animals; }
	}

	public static int CurrentWheat
	{
		get { return Instance.values.crops[0]; }
		set
		{
			Instance.values.crops[0] = value;
			UpdateValues();
		}
	}

	public static int OffsetedWheat
	{
		get
		{
			return 
				CurrentWheat + 
				Instance.offset.crops[0];
		}
	}
	
	public static int CurrentOat
	{
		get { return Instance.values.crops[1]; }
		set
		{
			Instance.values.crops[1] = value;
			UpdateValues();
		}
	}

	public static int OffsetedOat
	{
		get
		{
			return 
				CurrentOat + 
				Instance.offset.crops[1];
		}
	}

	public static int CurrentWheatSeed
	{
		get { return Instance.values.cropSeeds[0]; }
		set
		{
			Instance.values.cropSeeds[0] = value;
			UpdateValues();
		}
	}

	public static int OffsetedWheatSeed
	{
		get
		{
			return 
				CurrentWheatSeed + 
				Instance.offset.cropSeeds[0];
		}
	}

	public static int CurrentOatSeed
	{
		get { return Instance.values.cropSeeds[1]; }
		set
		{
			Instance.values.cropSeeds[1] = value;
			UpdateValues();
		}
	}

	public static int OffsetedOatSeed
	{
		get
		{
			return
				CurrentOatSeed +
				Instance.offset.cropSeeds[1];
		}
	}

	public static int CurrentFertiliser
	{
		get { return Instance.values.fertiliser; }
		set
		{
			Instance.values.fertiliser = value;
			UpdateValues();
		}
	}

	public static int OffsetedFertiliser
	{
		get
		{
			return
				CurrentFertiliser +
				Instance.offset.fertiliser;
		}
	}

	public static int CurrentFamily
	{
		get { return Instance.values.family; }
		set
		{
			Instance.values.family = value;
			UpdateValues();
		}
	}

	public static int OffsetedFamily
	{
		get
		{
			return
				CurrentFamily +
				Instance.offset.family;
		}
	}

	public static int CurrentChicken
	{
		get { return Instance.values.animals[0]; }
		set
		{
			Instance.values.animals[0] = value;
			UpdateValues();
		}
	}

	public static int OffsetedChicken
	{
		get
		{
			return
				CurrentChicken +
				Instance.offset.animals[0];
		}
	}

	public static int CurrentOx
	{
		get { return Instance.values.animals[1]; }
		set
		{
			Instance.values.animals[1] = value;
			UpdateValues();
		}
	}

	public static int OffsetedOx
	{
		get
		{
			return
				CurrentOx +
				Instance.offset.animals[1];
		}
	}

	public static int CurrentFamilyHunger
	{
		get { return Instance.values.familyHungeryPoint; }
		set
		{
			Instance.values.familyHungeryPoint = value;
			UpdateValues();
		}
	}

	public static int OffsetedFamilyHunger
	{
		get { return CurrentFamilyHunger + Instance.offset.familyHungeryPoint; }
	}

	public static int CurrentAnimalHunger
	{
		get { return Instance.values.animalHungeryPoint; }
		set
		{
			Instance.values.animalHungeryPoint = value;
			UpdateValues();
		}
	}

	public static int OffsetedAnimalHunger
	{
		get { return CurrentAnimalHunger + Instance.offset.animalHungeryPoint; }
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
		data.values = Values;
		data.offset = EventOffset;
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
			return;
		}

		SettingGame(game);
	}

	void SettingGame(GameData data)
	{
		TimeManager.Date = data.Date;
		values = data.values;
		offset = data.offset;
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
