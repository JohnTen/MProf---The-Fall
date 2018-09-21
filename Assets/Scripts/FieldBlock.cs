using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.Interactables;

public class FieldBlock : MonoInteractable
{
	[SerializeField] GameObject cropModel;

	Crop currentCrop;
	int plantingDate;
	int currentGrowingPeriod;

	public override void StartInteracting()
	{
		if (currentCrop == null)
		{
			WorldUI.MoveUIByWorldPosition(WorldUI.CropMenu.transform, this.transform.position);
			WorldUI.CropMenu.OpenMenu();
			WorldUI.CropMenu.OnCropChosed += CropMenu_OnCropChosed;
			return;
		}

		var neededTime = currentCrop.growingPeriods.Length - currentGrowingPeriod;
		if (neededTime > 0)
		{
			print("This crop needs " + neededTime + " more week(s) to grow up.");
			return;
		}

		GlobalValues.CurrentFood += currentCrop.foodValue;
		Destroy(cropModel);
		currentGrowingPeriod = 0;
		currentCrop = null;

		base.StartInteracting();
	}

	private void CropMenu_OnCropChosed(Crop crop)
	{
		WorldUI.CropMenu.CloseMenu();
		if (currentCrop != null)
		{
			Debug.LogWarning(name + " Already planted " + crop.name);
			return;
		}
		Plant(crop);
	}

	public override void StopInteracting() { }

	public void Plant(Crop crop)
	{
		currentCrop = crop;
		currentGrowingPeriod = 0;
		plantingDate = TimeManager.Date;

		cropModel = Instantiate(currentCrop.modelsForGrowingPeriod[currentGrowingPeriod]);
		cropModel.transform.position = this.transform.position;
		cropModel.transform.SetParent(this.transform);
	}

	private void Awake()
	{
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
	}

	private void TimeManager_OnTimePassed(int date)
	{
		if (currentCrop == null) return;
		if (currentGrowingPeriod >= currentCrop.growingPeriods.Length) return;
		if ((date - plantingDate) < currentCrop.growingPeriods[currentGrowingPeriod])
			return;

		plantingDate = date;
		currentGrowingPeriod++;

		Destroy(cropModel);
		cropModel = Instantiate(currentCrop.modelsForGrowingPeriod[currentGrowingPeriod]);
		cropModel.transform.position = this.transform.position;
		cropModel.transform.SetParent(this.transform);
	}
}
