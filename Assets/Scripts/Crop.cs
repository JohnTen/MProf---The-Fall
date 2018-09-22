using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Crop
{
	public string name;
	public int index;
	public int foodValue;
	public int growingPeriod;
	public float dropSeedPossibility;
	public int dropSeedNumber;
	public GameObject[] modelsForGrowingPeriod;

	public Crop() { }
	public Crop(Crop crop)
	{
		name = crop.name;
		index = crop.index;
		foodValue = crop.foodValue;
		growingPeriod = crop.growingPeriod;
		dropSeedPossibility = crop.dropSeedPossibility;
		dropSeedNumber = crop.dropSeedNumber;
		modelsForGrowingPeriod = new GameObject[crop.modelsForGrowingPeriod.Length];

		for (int i = 0; i < modelsForGrowingPeriod.Length; i++)
		{
			modelsForGrowingPeriod[i] = crop.modelsForGrowingPeriod[i];
		}
	}
}
