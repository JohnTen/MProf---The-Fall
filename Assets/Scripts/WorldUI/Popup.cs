using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class Popup : MonoSingleton<Popup>
{
	[SerializeField] PopupToken popupPrefab;

	public static void Pop(string text, Vector3 position)
	{
		var popup = Instantiate(Instance.popupPrefab);
		WorldUI.AddedUIElement(popup.transform as RectTransform);
		WorldUI.MoveUIByWorldPosition(popup.transform, position);
		popup.Text.text = text;
	}

	public static void Pop(string text, Vector3 position, Color color)
	{
		var popup = Instantiate(Instance.popupPrefab);
		WorldUI.AddedUIElement(popup.transform as RectTransform);
		WorldUI.MoveUIByWorldPosition(popup.transform, position);
		popup.Text.text = text;

		var alphaKeys = popup.Color.alphaKeys;
		var colorKeys = popup.Color.colorKeys;
		for (int i = 0; i < colorKeys.Length; i++)
		{
			colorKeys[i].color = color;
		}

		popup.Color.SetKeys(colorKeys, alphaKeys);
	}
}
