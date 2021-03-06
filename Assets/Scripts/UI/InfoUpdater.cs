﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUpdater : MonoBehaviour
{
	[SerializeField] bool autoUpdate;
	[SerializeField] Text dateText;
	[SerializeField] GameObject forcedTaxObject;
	[SerializeField] Text forcedTaxText;
	[SerializeField] Text taxText;
	[SerializeField] Text wheatText;
	[SerializeField] Text oatText;
	[SerializeField] Text milkText;
	[SerializeField] GameObject[] mechanRefs;

	private void Awake()
	{
		GameDataManager.OnValueChanged += GlobalValues_OnvalueChanged;
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
		
		UpdateInfo();
	}

	private void Update()
	{
		if (autoUpdate) UpdateInfo();
	}

	private void OnDestroy()
	{
		GameDataManager.OnValueChanged -= GlobalValues_OnvalueChanged;
		TimeManager.OnTimePassed -= TimeManager_OnTimePassed;
	}

	public void UpdateInfo()
	{
		GameDataManager.GameValues.CalculateModifiedValue();
		if (GameDataManager.GameValues[GameValueType.ForcedTaxRate] != 0)
		{
			forcedTaxObject.SetActive(true);
			forcedTaxText.text = (GameDataManager.GameValues[GameValueType.ForcedTaxRate] * 100) + "%";
		}
		else
		{
			forcedTaxObject.SetActive(false);
		}

		for (int i = 0; i < mechanRefs.Length; i++)
		{
			mechanRefs[i].SetActive(CheckList.Instance.gather[i] > 0);
		}

		dateText.text			= (GameDataManager.MaxWeek - TimeManager.Date).ToString();
		taxText.text			= (GameDataManager.GameValues[GameValueType.TaxRate] * 100).ToString() + "%";
		wheatText.text			= GameDataManager.CurrentWheat.ToString();
		oatText.text			= GameDataManager.CurrentOat.ToString();
		milkText.text = GameDataManager.GameValues[GameValueType.Milks].ToString();
	}

	private void GlobalValues_OnvalueChanged()
	{
		UpdateInfo();
	}

	private void TimeManager_OnTimePassed(int date)
	{
		dateText.text = (GameDataManager.MaxWeek - date).ToString();
	}
}
