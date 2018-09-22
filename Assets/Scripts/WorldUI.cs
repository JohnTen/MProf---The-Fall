using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class WorldUI : MonoSingleton<WorldUI>
{
	[SerializeField] ButtonMenu cropMenu;
	[SerializeField] ButtonMenu fertiliserMenu;

	public static ButtonMenu CropMenu
	{
		get { return Instance.cropMenu; }
	}

	public static ButtonMenu FertiliserMenu
	{
		get { return Instance.fertiliserMenu; }
	}

	public static void CloseAllMenu()
	{
		CropMenu.CloseMenu();
		FertiliserMenu.CloseMenu();
	}

	public static void MoveUIByPixelPosition(Transform ui, Vector3 pixelPosition)
	{
		pixelPosition.z = (Instance.transform.position - GlobalValues.MainCamera.transform.position).z;
		var position = GlobalValues.MainCamera.ScreenToWorldPoint(pixelPosition);

		ui.transform.position = position;
	}

	public static void MoveUIByWorldPosition(Transform ui, Vector3 worldPosition)
	{
		MoveUIByPixelPosition(ui, GlobalValues.MainCamera.WorldToScreenPoint(worldPosition));
	}
}
