using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class GlobalValues : MonoSingleton<GlobalValues>
{
	[SerializeField] Camera mainCamera;
	[SerializeField] int currentFood;

	public static event Action<int> OnFoodChanged;

	public static Camera MainCamera
	{
		get
		{
			if (Instance.mainCamera == null)
				Instance.mainCamera = Camera.main;

			return Instance.mainCamera;
		}
	}

	public static int CurrentFood
	{
		get { return Instance.currentFood; }
		set
		{
			Instance.currentFood = value;
			if (OnFoodChanged != null)
				OnFoodChanged.Invoke(CurrentFood);
		}
	}
}
