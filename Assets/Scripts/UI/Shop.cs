﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[SerializeField] Text currentWheat;
	[SerializeField] Text currentBarley;
	[SerializeField] Text currentMilk;
	[SerializeField] Text typeLabel;
	[SerializeField] Text nameLabel;
	[SerializeField] Text wheatPrice;
	[SerializeField] Text barleyPrice;
	[SerializeField] Text milkPrice;
	[SerializeField] Image picture;
	[SerializeField] Button purchaseButton;

	int currentIndex;

	public void UpdateStock()
	{
		var good = GameDatabase.Instance.merchandiseList[currentIndex];

		currentWheat.text = GameDataManager.GameValues[GameValueType.NumberOfWheat].ToString();
		currentBarley.text = GameDataManager.GameValues[GameValueType.NumberOfOat].ToString();
		currentMilk.text = GameDataManager.GameValues[GameValueType.Milks].ToString();


		typeLabel.text = good.familyMember != FamilyType.None ? "Gifts" : "Supplies";

		nameLabel.text = good.name;

		wheatPrice.text = good.requiredWheat.ToString();
		barleyPrice.text = good.requiredOat.ToString();
		milkPrice.text = good.requiredMilk.ToString();

		picture.sprite = good.pic;

		purchaseButton.interactable =
			(good.familyMember != FamilyType.None || CheckList.Instance.gather[currentIndex] <= 0) &&
			GameDataManager.GameValues[GameValueType.NumberOfWheat] >= good.requiredWheat &&
			GameDataManager.GameValues[GameValueType.NumberOfOat] >= good.requiredOat &&
			GameDataManager.GameValues[GameValueType.Milks] >= good.requiredMilk;
	}

	public void NextMechandise()
	{
		currentIndex++;
		if (currentIndex >= GameDatabase.Instance.merchandiseList.Count)
			currentIndex = 0;

		UpdateStock();
	}

	public void PreviousMechandise()
	{
		currentIndex--;
		if (currentIndex < 0)
			currentIndex = GameDatabase.Instance.merchandiseList.Count - 1;

		UpdateStock();
	}

	public void Buying()
	{
		var good = GameDatabase.Instance.merchandiseList[currentIndex];
		GameDataManager.GameValues[GameValueType.NumberOfWheat] -= good.requiredWheat;
		GameDataManager.GameValues[GameValueType.NumberOfOat] -= good.requiredOat;
		GameDataManager.GameValues[GameValueType.Milks] -= good.requiredMilk;
		GameDataManager.GameValues.CalculateModifiedValue();
		GameDataManager.UpdateValues();
		UpdateStock();

		if (good.familyMember == FamilyType.None)
		{
			CheckList.Instance.gather[currentIndex]++;
			GameDataManager.UpdateValues();
		}
		else switch (good.familyMember)
		{
			case FamilyType.None: break;
			case FamilyType.PC:
				FamilyManager.FamilyMembers[0].mentalHealth += good.sanityBoost;
				break;
			case FamilyType.Wife:
				FamilyManager.FamilyMembers[1].mentalHealth += good.sanityBoost;
				break;
			case FamilyType.Daughter:
				FamilyManager.FamilyMembers[2].mentalHealth += good.sanityBoost;
				break;
			case FamilyType.Son:
				FamilyManager.FamilyMembers[3].mentalHealth += good.sanityBoost;
				break;
		}
		
		purchaseButton.interactable =
			(good.familyMember != FamilyType.None || CheckList.Instance.gather[currentIndex] <= 0) &&
			GameDataManager.GameValues[GameValueType.NumberOfWheat] >= good.requiredWheat &&
			GameDataManager.GameValues[GameValueType.NumberOfOat] >= good.requiredOat &&
			GameDataManager.GameValues[GameValueType.Milks] >= good.requiredMilk;
	}

	private void Start()
	{
		UpdateStock();
	}
}
