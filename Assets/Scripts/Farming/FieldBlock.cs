﻿using System.Collections;
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
	[SerializeField] MeshRenderer[] plotRenderers;

	[SerializeField] FieldBlockStatus status;
	[SerializeField] Material afterPlougheMaterial;
	[SerializeField] string plantingSoundLabel = "Planting";
	[SerializeField] string harvestingSoundLabel = "Harvesting";

	Material[] beforePlougheMaterial;

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

	private void WaitForPloughingMinigame(float result)
	{
		status.fouled = result == 0;
		if (status.fouled)
		{
			status.lastPlantedCrop = null;
			Clear();
			return;
		}

		for (int i = 0; i < plotRenderers.Length; i++)
		{
			plotRenderers[i].material = afterPlougheMaterial;
		}

		var value = result * 1.2f;
		if (value > 1)
			Popup.Pop("Product + " + Mathf.Round((value - 1) * 10000)/100 + "%", transform.position + Vector3.up * 3, Color.green);
		else if (value < 1)
			Popup.Pop("Product - " + Mathf.Round((value - 1) * -10000)/100 + "%", transform.position + Vector3.up * 3, Color.red);
		else
			Popup.Pop("Product no change", transform.position + Vector3.up * 3);
		
		status.lastPlantedCrop = status.currentCrop;
		status.lastPlantedCrop.foodValue = (int)(status.lastPlantedCrop.foodValue * value);

		CreateCropModel();

		isActivated = true;
		InvokeActivated();
		onActivated.Invoke();
	}

	private void WaitForHarvestingMinigame(float result)
	{
		status.currentCrop.foodValue = Mathf.RoundToInt(status.currentCrop.foodValue * (0.5f + 0.5f * result));

		GameDataManager.ModifyCropNumber(status.currentCrop.index, Mathf.RoundToInt(status.currentCrop.foodValue * GameDataManager.GameValues[GameValueType.CropProduction]));
		GameDataManager.UpdateValues();
		SoundManager.Play(harvestingSoundLabel);

		var crop = status.currentCrop.name;
		if (status.currentCrop.foodValue == 0)
			Popup.Pop(crop + " + " + status.currentCrop.foodValue, transform.position + Vector3.up * 3);
		else
			Popup.Pop(crop + " + " + status.currentCrop.foodValue, transform.position + Vector3.up * 3, Color.green);

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
					WaitForPloughingMinigame(1);
				}, 
				() => FieldManager.Instance.StartPlantMinigame(crop.index, WaitForPloughingMinigame)
			);
		}
		else
		{
			FieldManager.Instance.StartPlantMinigame(crop.index, WaitForPloughingMinigame);
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

		FieldManager.Instance.StartHarvestMinigame(status.currentCrop.index, WaitForHarvestingMinigame);
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

		for (int i = 0; i < plotRenderers.Length; i++)
		{
			plotRenderers[i].material = beforePlougheMaterial[i];
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
		beforePlougheMaterial = new Material[plotRenderers.Length];

		for (int i = 0; i < plotRenderers.Length; i++)
		{
			beforePlougheMaterial[i] = plotRenderers[i].material;
		}
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
