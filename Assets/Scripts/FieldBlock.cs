using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Interactables;

public class FieldBlock : MonoInteractable
{
	[SerializeField] GameObject cropModel;

	Crop currentCrop;
	int currentGrowingPeriod;
	bool fertilised;

	public Crop CurrentCrop
	{
		get { return currentCrop; }
	}

	public override void StartInteracting()
	{
		if (currentCrop == null &&
			(GlobalValues.CurrentWheatSeed > 0 ||
			GlobalValues.CurrentOatSeed > 0))
		{
			WorldUI.MoveUIByWorldPosition(WorldUI.CropMenu.transform, this.transform.position);
			WorldUI.CropMenu.OpenMenu();
			WorldUI.CropMenu.OnChosed += CropMenu_OnCropChosed;
			return;
		}
		else if (
			currentCrop.growingPeriod > currentGrowingPeriod &&
			GlobalValues.CurrentFertiliser > 0 &&
			!fertilised)
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
		if (currentCrop == null)
		{
			Debug.LogError(name + " has no crop planted ");
			return;
		}

		GlobalValues.CurrentFertiliser--;

		fertilised = true;
		currentCrop.foodValue *= 2;
	}

	private void CropMenu_OnCropChosed(int index)
	{
		var crop = DataBase.Instance.cropList[index];
		WorldUI.CropMenu.CloseMenu();
		if (currentCrop != null)
		{
			Debug.LogError(name + " Already planted " + crop.name);
			return;
		}
		Plant(crop);
	}

	public override void StopInteracting() { }

	public void Plant(Crop crop)
	{
		if (GlobalValues.CurrentCropSeeds[crop.index] <= 0)
		{
			Debug.Log("You are out of seeds.");
			return;
		}
		GlobalValues.CurrentCropSeeds[crop.index]--;
		GlobalValues.UpdateValues();

		currentCrop = new Crop(crop);
		currentGrowingPeriod = 0;

		cropModel = Instantiate(currentCrop.modelsForGrowingPeriod[currentGrowingPeriod]);
		cropModel.transform.position = this.transform.position;
		cropModel.transform.SetParent(this.transform);

		isActivated = true;
		InvokeActivated();
		onActivated.Invoke();
	}

	public void Harvest()
	{
		if (currentCrop == null ||
			currentCrop.growingPeriod > currentGrowingPeriod)
			return;

		if (Random.value < currentCrop.dropSeedPossibility)
			GlobalValues.CurrentCropSeeds[currentCrop.index] += currentCrop.dropSeedNumber;
		GlobalValues.CurrentCrops[currentCrop.index] += currentCrop.foodValue;
		GlobalValues.UpdateValues();

		Clear();
	}

	public void Clear()
	{
		if (cropModel != null)
			Destroy(cropModel);

		fertilised = false;
		currentGrowingPeriod = 0;
		currentCrop = null;
	}

	private void Awake()
	{
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
	}

	private void TimeManager_OnTimePassed(int date)
	{
		if (currentCrop == null) return;
		if (currentGrowingPeriod >= currentCrop.growingPeriod) return;
		
		currentGrowingPeriod++;

		Destroy(cropModel);
		cropModel = Instantiate(currentCrop.modelsForGrowingPeriod[currentGrowingPeriod]);
		cropModel.transform.position = this.transform.position;
		cropModel.transform.SetParent(this.transform);

		if (currentGrowingPeriod >= currentCrop.growingPeriod)
		{
			isActivated = false;
			InvokeDeactivated();
			onDeactivated.Invoke();
		}
	}
}
