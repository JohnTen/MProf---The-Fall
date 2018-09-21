using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Crop
{
	public string name;
	public int foodValue;
	public int[] growingPeriods;
	public GameObject[] modelsForGrowingPeriod;

	public Crop() { }
	public Crop(Crop crop)
	{
		foodValue = crop.foodValue;
		growingPeriods = new int[crop.growingPeriods.Length];
		modelsForGrowingPeriod = new GameObject[crop.modelsForGrowingPeriod.Length];

		for (int i = 0; i < growingPeriods.Length; i++)
		{
			growingPeriods[i] = crop.growingPeriods[i];
		}

		for (int i = 0; i < modelsForGrowingPeriod.Length; i++)
		{
			modelsForGrowingPeriod[i] = crop.modelsForGrowingPeriod[i];
		}
	}
}
