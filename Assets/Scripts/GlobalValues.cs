using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class GlobalValues : MonoSingleton<GlobalValues>
{
	[SerializeField] Camera mainCamera;

	public static Camera MainCamera
	{
		get
		{
			if (Instance.mainCamera == null)
				Instance.mainCamera = Camera.main;

			return Instance.mainCamera;
		}
	}
}
