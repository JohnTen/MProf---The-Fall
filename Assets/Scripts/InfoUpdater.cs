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
		GlobalValues.OnValueChanged += GlobalValues_OnvalueChanged;
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
		wheatText.text			= GlobalValues.OffsetedWheat.ToString();
		oatText.text			= GlobalValues.OffsetedOat.ToString();
		wheatSeedText.text		= GlobalValues.OffsetedWheatSeed.ToString();
		oatSeedText.text		= GlobalValues.OffsetedOatSeed.ToString();
		fertiliserText.text		= GlobalValues.OffsetedFertiliser.ToString();
		familyText.text			= GlobalValues.OffsetedFamily.ToString();
		chickenText.text		= GlobalValues.OffsetedChicken.ToString();
		oxText.text				= GlobalValues.OffsetedOx.ToString();
		fHungerText.text		= GlobalValues.OffsetedFamilyHunger.ToString();
		aHungerText.text		= GlobalValues.OffsetedAnimalHunger.ToString();
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
