using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class WorldUI : MonoSingleton<WorldUI>
{
	[SerializeField] CropMenu cropMenu;

	public static CropMenu CropMenu
	{
		get { return Instance.cropMenu; }
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
