using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class WorldUI : MonoSingleton<WorldUI>
{
	[SerializeField] ButtonMenu cropMenu;

	public static ButtonMenu CropMenu
	{
		get { return Instance.cropMenu; }
	}

	public static void CloseAllMenu()
	{
		CropMenu.CloseMenu();
	}

	public static void AddedUIElement(RectTransform obj)
	{
		obj.SetParent(Instance.transform);
		obj.localScale = Vector3.one;
		obj.localRotation = Quaternion.identity;
	}

	public static void MoveUIByPixelPosition(Transform ui, Vector3 pixelPosition)
	{
		pixelPosition.z = (Instance.transform.position - GameDataManager.MainCamera.transform.position).z;
		var position = GameDataManager.MainCamera.ScreenToWorldPoint(pixelPosition);

		ui.transform.position = position;
	}

	public static void MoveUIByWorldPosition(Transform ui, Vector3 worldPosition)
	{
		MoveUIByPixelPosition(ui, GameDataManager.MainCamera.WorldToScreenPoint(worldPosition));
	}
}
