using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Interactables;

[System.Serializable]
public struct FieldBlockStatus
{
	public bool plantedCrop;
	public Crop currentCrop;
	public int currentGrowingPeriod;
	public bool fertilised;

	public FieldBlockStatus(FieldBlockStatus status)
	{
		plantedCrop				= status.plantedCrop;
		currentCrop				= new Crop(status.currentCrop);
		currentGrowingPeriod	= status.currentGrowingPeriod;
		fertilised				= status.fertilised;
	}
}

public class FieldBlock : MonoInteractable
{
	[SerializeField] GameObject cropModel;
	[SerializeField] GameObject fertiliserModel;

	[SerializeField] FieldBlockStatus status;

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
		if (!status.plantedCrop)
		{
			if (
			GameDataManager.CurrentWheatSeed > 0 ||
			GameDataManager.CurrentOatSeed > 0)
			{
				WorldUI.MoveUIByWorldPosition(WorldUI.CropMenu.transform, this.transform.position);
				WorldUI.CropMenu.OpenMenu();
				WorldUI.CropMenu.OnChosed += CropMenu_OnCropChosed;
				return;
			}
		}
		else if (
			status.currentGrowingPeriod == 0 &&
			GameDataManager.CurrentFertiliser > 0 &&
			!status.fertilised)
		{
			WorldUI.MoveUIByWorldPosition(WorldUI.FertiliserMenu.transform, this.transform.position);
			WorldUI.FertiliserMenu.OpenMenu();
			WorldUI.FertiliserMenu.OnChosed += FertiliserMenu_OnChosed; ;
			return;
		}
		else
		{
			Harvest();
		}

		base.StartInteracting();
	}

	private void FertiliserMenu_OnChosed(int index)
	{
		WorldUI.FertiliserMenu.CloseMenu();
		if (!status.plantedCrop)
		{
			Debug.LogError(name + " has no crop planted ");
			return;
		}

		GameDataManager.CurrentFertiliser--;

		status.fertilised = true;
		status.currentCrop.foodValue *= 2;
		fertiliserModel = Instantiate(GameDatabase.Instance.normalFertiliserModel);
		fertiliserModel.transform.position = this.transform.position;
		fertiliserModel.transform.SetParent(this.transform);
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
		Plant(crop);
	}

	public override void StopInteracting() { }

	public void Plant(Crop crop)
	{
		if (GameDataManager.GetCropSeedNumbr(crop.index) <= 0)
		{
			Debug.Log("You are out of seeds.");
			return;
		}
		GameDataManager.ModifyCropSeedNumbr(crop.index, -1);
		GameDataManager.UpdateValues();

		status.plantedCrop = true;
		status.currentCrop = new Crop(crop);
		status.currentGrowingPeriod = 0;

		CreateCropModel();

		isActivated = true;
		InvokeActivated();
		onActivated.Invoke();
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
		
		if (Random.value < status.currentCrop.dropSeedPossibility * GameDataManager.GameValues[GameValueType.SeedDroppingRate])
			GameDataManager.ModifyCropSeedNumbr(status.currentCrop.index, Mathf.RoundToInt(status.currentCrop.dropSeedNumber * GameDataManager.GameValues[GameValueType.SeedProduction]));
		GameDataManager.ModifyCropNumber(status.currentCrop.index, Mathf.RoundToInt(status.currentCrop.foodValue * GameDataManager.GameValues[GameValueType.CropProduction]));
		GameDataManager.UpdateValues();

		Clear();
	}

	public void Clear()
	{
		if (cropModel != null)
			Destroy(cropModel);

		if (fertiliserModel != null)
			Destroy(fertiliserModel);

		status.plantedCrop			= false;
		status.fertilised			= false;
		status.currentCrop			= null;
		status.currentGrowingPeriod = 0;
	}

	private void CreateCropModel()
	{
		cropModel = Instantiate(GameDatabase.Instance.GetGrowingModel(status.currentCrop.index, status.currentGrowingPeriod));
		cropModel.transform.position = this.transform.position;
		cropModel.transform.SetParent(this.transform);
	}

	private void Awake()
	{
		status = new FieldBlockStatus();
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
	}

	private void TimeManager_OnTimePassed(int date)
	{
		if (!status.plantedCrop) return;
		if (status.currentGrowingPeriod >= status.currentCrop.growingPeriod) return;

		status.currentGrowingPeriod++;

		Destroy(cropModel);
		CreateCropModel();

		if (status.currentGrowingPeriod >= status.currentCrop.growingPeriod)
		{
			isActivated = false;
			InvokeDeactivated();
			onDeactivated.Invoke();
		}
	}
}
