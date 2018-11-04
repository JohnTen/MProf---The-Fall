using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Interactables;

[System.Serializable]
public struct FieldBlockStatus
{
	public bool fouled;
	public bool plantedCrop;
	public Crop currentCrop;
	public Crop lastPlantedCrop;
	public int currentGrowingPeriod;

	public FieldBlockStatus(FieldBlockStatus status)
	{
		fouled = status.fouled;
		plantedCrop = status.plantedCrop;
		currentCrop = new Crop(status.currentCrop);
		lastPlantedCrop = new Crop(status.lastPlantedCrop);
		currentGrowingPeriod = status.currentGrowingPeriod;
	}
}

public class FieldBlock : MonoInteractable
{
	[SerializeField] GameObject[] cropModels;
	[SerializeField] GameObject[] fertiliserModels;
	[SerializeField] Transform[] subFields;

	[SerializeField] FieldBlockStatus status;
	[SerializeField] string plantingSoundLabel = "Planting";
	[SerializeField] string harvestingSoundLabel = "Harvesting";

	public FieldBlockStatus Status
	{
		get { return status; }
		set
		{
			Clear();
			status = value;

			if (status.plantedCrop)
			{
				ForcePlant(status.currentCrop, status.currentGrowingPeriod);
			}
		}
	}

	public override void StartInteracting()
	{
		if (status.fouled)
			return;

		if (!status.plantedCrop)
		{
			WorldUI.MoveUIByWorldPosition(WorldUI.CropMenu.transform, this.transform.position);
			WorldUI.CropMenu.OpenMenu();
			WorldUI.CropMenu.OnChosed += CropMenu_OnCropChosed;
			return;
		}
		else
		{
			Harvest();
		}

		base.StartInteracting();
	}

	private void CropMenu_OnCropChosed(int index)
	{
		var crop = GameDatabase.Instance.cropList[index];
		WorldUI.CropMenu.CloseMenu();
		if (status.plantedCrop)
		{
			Debug.LogError(name + " Already planted " + crop.name);
			return;
		}

		if (status.lastPlantedCrop != null &&
			status.lastPlantedCrop.index == index &&
			status.lastPlantedCrop.index != 0)
		{
			MessageBox.DisplayMessage("You cannot plant the same crop twice in a row.");
			return;
		}

		Plant(crop);
	}

	public override void StopInteracting() { }

	private void WaitForFarmingMinigame(bool result)
	{
		status.fouled = !result;
		if (status.fouled)
		{
			status.lastPlantedCrop = null;
			Clear();
			return;
		}
		status.lastPlantedCrop = status.currentCrop;

		CreateCropModel();

		isActivated = true;
		InvokeActivated();
		onActivated.Invoke();
	}

	private void WaitForHarvestingMinigame(bool result)
	{
		if (!result)
			status.currentCrop.foodValue /= 2;

		GameDataManager.ModifyCropNumber(status.currentCrop.index, Mathf.RoundToInt(status.currentCrop.foodValue * GameDataManager.GameValues[GameValueType.CropProduction]));
		GameDataManager.UpdateValues();
		SoundManager.Play(harvestingSoundLabel);

		Clear();
	}

	public void Plant(Crop crop)
	{
		SoundManager.Play(plantingSoundLabel);

		status.plantedCrop = true;
		status.currentCrop = new Crop(crop);
		status.currentGrowingPeriod = 0;

		if (GameDataManager.GameValues[GameValueType.Fertiliser] > 0f)
		{
			MessageBox.DisplayMessage(
				"", 
				"Do you want to skip the minigame by using one fertiliser?", 
				() => 
				{
					GameDataManager.GameValues[GameValueType.Fertiliser]--;
					GameDataManager.UpdateValues();
					WaitForFarmingMinigame(true);
				}, 
				() => FieldManager.Instance.StartPlantMinigame(crop.index, WaitForFarmingMinigame)
			);
		}
		else
		{
			FieldManager.Instance.StartPlantMinigame(crop.index, WaitForFarmingMinigame);
		}
	}

	public void ForcePlant(Crop crop, int growingPeriod)
	{
		status.plantedCrop = true;
		status.currentCrop = new Crop(crop);
		status.currentGrowingPeriod = growingPeriod;

		CreateCropModel();

		if (status.currentGrowingPeriod >= status.currentCrop.growingPeriod)
			return;

		isActivated = true;
		InvokeActivated();
		onActivated.Invoke();
	}

	public void Harvest()
	{
		if (!status.plantedCrop ||
			status.currentCrop.growingPeriod > status.currentGrowingPeriod)
			return;

		FieldManager.Instance.StartHarvestMinigame(WaitForHarvestingMinigame);
	}

	public void Clear()
	{
		for (int i = 0; i < cropModels.Length; i++)
		{
			if (cropModels[i] != null)
				Destroy(cropModels[i]);
		}

		for (int i = 0; i < fertiliserModels.Length; i++)
		{
			if (fertiliserModels[i] != null)
				Destroy(fertiliserModels[i]);
		}

		status.plantedCrop = false;
		status.currentCrop = null;
		status.currentGrowingPeriod = 0;
	}

	private void CreateCropModel()
	{
		for (int i = 0; i < subFields.Length; i++)
		{
			cropModels[i] = Instantiate(GameDatabase.Instance.GetGrowingModel(status.currentCrop.index, status.currentGrowingPeriod));
			cropModels[i].transform.position = subFields[i].position;
			cropModels[i].transform.SetParent(subFields[i]);
		}
	}

	private void Awake()
	{
		status = new FieldBlockStatus();
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
		cropModels = new GameObject[subFields.Length];
		fertiliserModels = new GameObject[subFields.Length];
	}

	private void TimeManager_OnTimePassed(int date)
	{
		if (status.fouled) status.fouled = false;

		if (!status.plantedCrop) return;
		if (status.currentGrowingPeriod >= status.currentCrop.growingPeriod) return;

		status.currentGrowingPeriod++;

		for (int i = 0; i < cropModels.Length; i++)
		{
			if (cropModels[i] != null)
				Destroy(cropModels[i]);
		}
		CreateCropModel();

		if (status.currentGrowingPeriod >= status.currentCrop.growingPeriod)
		{
			isActivated = false;
			InvokeDeactivated();
			onDeactivated.Invoke();
		}
	}
}
