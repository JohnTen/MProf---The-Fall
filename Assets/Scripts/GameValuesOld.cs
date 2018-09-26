using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameValuesOld
{
	public int[] crops;
	public int[] cropSeeds;
	public int[] animals;
	public int family;
	public int fertiliser;
	public int familyHungeryPoint;
	public int animalHungeryPoint;

	public GameValuesOld()
	{
		crops = new int[2];
		cropSeeds = new int[2];
		animals = new int[2];
		family = 0;
	}

	public GameValuesOld(GameValuesOld value)
	{
		crops = new int[value.crops.Length];
		for (int i = 0; i < crops.Length; i++)
		{
			crops[i] = value.crops[i];
		}

		cropSeeds = new int[value.cropSeeds.Length];
		for (int i = 0; i < cropSeeds.Length; i++)
		{
			cropSeeds[i] = value.cropSeeds[i];
		}

		animals = new int[value.animals.Length];
		for (int i = 0; i < animals.Length; i++)
		{
			animals[i] = value.animals[i];
		}

		family = value.family;
	}

	public void Clear()
	{
		for (int i = 0; i < crops.Length; i++)
		{
			crops[i] = 0;
		}

		for (int i = 0; i < cropSeeds.Length; i++)
		{
			cropSeeds[i] = 0;
		}

		for (int i = 0; i < animals.Length; i++)
		{
			animals[i] = 0;
		}

		family = 0;
	}
}
