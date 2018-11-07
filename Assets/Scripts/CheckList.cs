using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

/// <summary>
/// A very very dirty, naughty class...
/// </summary>
public class CheckList : MonoSingleton<CheckList>
{
	/// <summary>
	/// And some very very dirty, naughty little things...
	/// </summary>
	public int[] gather = new int[7];

	[SerializeField] Text wheatTexts;
	[SerializeField] Text barleyTexts;
	[SerializeField] Text milkTexts;
	[SerializeField] Image[] checkBoxes;

	private void Start()
	{
		wheatTexts.text = gather[4].ToString() + "x Wheat";
		barleyTexts.text = gather[5].ToString() + "x Barley";
		milkTexts.text = gather[6].ToString() + "x Milk";
		UpdateCheckList();
		GameDataManager.OnValueChanged += UpdateCheckList;
	}

	public void UpdateCheckList()
	{
		for (int i = 0; i < gather.Length; i ++)
		{
			if (i < 4)
			{
				checkBoxes[i].enabled = gather[i] > 0;
			}
			else
			{
				switch (i)
				{
					case 4:
						checkBoxes[i].enabled = GameDataManager.GameValues[GameValueType.NumberOfWheat] >= gather[i];
						break;
					case 5:
						checkBoxes[i].enabled = GameDataManager.GameValues[GameValueType.NumberOfOat] >= gather[i];
						break;
					case 6:
						checkBoxes[i].enabled = GameDataManager.GameValues[GameValueType.Milks] >= gather[i];
						break;
				}
			}
		}
	}

	public bool IsCheckListFinished()
	{
		return
			gather[0] > 0 && gather[1] > 0 && gather[2] > 0 && gather[3] > 0 &&
			GameDataManager.GameValues[GameValueType.NumberOfWheat] >= gather[4] &&
			GameDataManager.GameValues[GameValueType.NumberOfOat] >= gather[5] &&
			GameDataManager.GameValues[GameValueType.Milks] >= gather[6];
	}
}
