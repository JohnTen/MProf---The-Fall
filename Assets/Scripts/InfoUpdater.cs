using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUpdater : MonoBehaviour
{
	[SerializeField] bool autoUpdate;
	[SerializeField] Text dateText;
	[SerializeField] Text wheatText;
	[SerializeField] Text oatText;
	[SerializeField] Text sonText;
	[SerializeField] Text daughterText;
	[SerializeField] Text wifeText;
	[SerializeField] Text oxText;
	[SerializeField] Text fHungerText;
	[SerializeField] Text aHungerText;

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

	public void UpdateInfo()
	{
		GameDataManager.GameValues.CalculateModifiedValue();
		dateText.text			= (GameDataManager.MaxWeek - TimeManager.Date).ToString();
		wheatText.text			= GameDataManager.CurrentWheat.ToString();
		oatText.text			= GameDataManager.CurrentOat.ToString();
		sonText.text			= GameDataManager.CurrentSon.ToString();
		daughterText.text		= GameDataManager.CurrentDaughter.ToString();
		wifeText.text			= GameDataManager.CurrentWife.ToString();
		oxText.text				= GameDataManager.CurrentOx.ToString();
		fHungerText.text		= GameDataManager.CurrentFamilyHunger.ToString();
		aHungerText.text		= GameDataManager.CurrentAnimalHunger.ToString();
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
