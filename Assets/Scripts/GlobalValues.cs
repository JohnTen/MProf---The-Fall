using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class GlobalValues : MonoSingleton<GlobalValues>
{
	[SerializeField] Camera mainCamera;
	[SerializeField] GameValues values;
	[SerializeField] GameValues offsets;

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
				Instance.offsets.crops[0];
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
				Instance.offsets.crops[1];
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
				Instance.offsets.cropSeeds[0];
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
				Instance.offsets.cropSeeds[1];
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
				Instance.offsets.fertiliser;
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
				Instance.offsets.family;
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
				Instance.offsets.animals[0];
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
				Instance.offsets.animals[1];
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
		get { return CurrentFamilyHunger + Instance.offsets.familyHungeryPoint; }
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
		get { return CurrentAnimalHunger + Instance.offsets.animalHungeryPoint; }
	}

	public static GameValues EventOffset
	{
		get { return Instance.offsets; }
	}

	public static void UpdateValues()
	{
		if (OnValueChanged != null)
			OnValueChanged.Invoke();
	}
}
