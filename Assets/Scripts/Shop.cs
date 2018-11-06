using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[SerializeField] Button[] buyingButtons;

	public void UpdateStock()
	{
		var list = GameDatabase.Instance.merchandiseList;
		for (int i = 0; i < list.Count; i++)
		{
			if (GameDataManager.GameValues[GameValueType.NumberOfWheat] < list[i].requiredWheat ||
				GameDataManager.GameValues[GameValueType.NumberOfOat] < list[i].requiredOat ||
				GameDataManager.GameValues[GameValueType.Milks] < list[i].requiredMilk)
				buyingButtons[i].interactable = false;
			else
				buyingButtons[i].interactable = true;
		}
	}

	public void Buying(int index)
	{
		var list = GameDatabase.Instance.merchandiseList;
		GameDataManager.GameValues[GameValueType.NumberOfWheat] -= list[index].requiredWheat;
		GameDataManager.GameValues[GameValueType.NumberOfOat] -= list[index].requiredOat;
		GameDataManager.GameValues[GameValueType.Milks] -= list[index].requiredMilk;
		UpdateStock();
	}
}
