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
	[SerializeField] Text wheatSeedText;
	[SerializeField] Text oatSeedText;
	[SerializeField] Text fertiliserText;
	[SerializeField] Text familyText;
	[SerializeField] Text chickenText;
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
		dateText.text			= TimeManager.Date.ToString();
		wheatText.text			= GameDataManager.OffsetedWheat.ToString();
		oatText.text			= GameDataManager.OffsetedOat.ToString();
		wheatSeedText.text		= GameDataManager.OffsetedWheatSeed.ToString();
		oatSeedText.text		= GameDataManager.OffsetedOatSeed.ToString();
		fertiliserText.text		= GameDataManager.OffsetedFertiliser.ToString();
		familyText.text			= GameDataManager.OffsetedFamily.ToString();
		chickenText.text		= GameDataManager.OffsetedChicken.ToString();
		oxText.text				= GameDataManager.OffsetedOx.ToString();
		fHungerText.text		= GameDataManager.OffsetedFamilyHunger.ToString();
		aHungerText.text		= GameDataManager.OffsetedAnimalHunger.ToString();
	}

	private void GlobalValues_OnvalueChanged()
	{
		UpdateInfo();
	}

	private void TimeManager_OnTimePassed(int date)
	{
		dateText.text = date.ToString();
	}
}
