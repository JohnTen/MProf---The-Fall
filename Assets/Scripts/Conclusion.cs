using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class Conclusion : MonoSingleton<Conclusion>
{
	[SerializeField] bool autoHarvest;
	[SerializeField] Color neutralColor = Color.black;
	[SerializeField] Color postiveColor = Color.green;
	[SerializeField] Color negativeColor = Color.red;
	
	[Header("Canvas of Windows")]
	[SerializeField] Canvas BasicCanvas;
	[SerializeField] Canvas TaxCanvas;
	[SerializeField] Canvas ConclusionCanvas;

	[Header("Tax Texts")]
	[SerializeField] string explanation =
@"    You have <i>{0}</i> bags of wheat and <i>{1}</i> bags of oat, you have to pay <i>{2}</i> bags of wheat and <i>{3}</i> bags of oat to your landlord, or... you can save some foods for your family by lying to your landlord...";
	[SerializeField] Text taxRateText;
	[SerializeField] Text explanationText;
	[SerializeField] Text paidWheatText;
	[SerializeField] Text paidOatText;
	[SerializeField] Slider wheatSlider;
	[SerializeField] Slider oatSlider;

	[Header("Conclusion Texts")]
	[SerializeField] Text remainedWheat;
	[SerializeField] Text remainedOat;
	[SerializeField] Text harvestedWheat;
	[SerializeField] Text harvestedOat;
	[SerializeField] Text wheatTax;
	[SerializeField] Text oatTax;
	[SerializeField] Text tendFamily;
	[SerializeField] Text tendAnimal;
	[SerializeField] Text eventWheat;
	[SerializeField] Text eventOat;
	[SerializeField] Text finalWheat;
	[SerializeField] Text finalOat;

	int paidWheat;
	int paidOat;
	int forcedPaidWheat;
	int forcedPaidOat;

	// When those event raised, the handlers can access to all the values they needed
	// so it is not necessary to use parameters
	public event Action OnConclusionStart;
	public event Action OnCalculateRemainedCrops;
	public event Action OnCalculateHarvcetedCrops;
	public event Action OnCalculateTaxes;
	public event Action OnCalculateFamilyConsumption;
	public event Action OnCalculateAnimalConsumption;
	public event Action OnCalculateEvents;
	public event Action OnConclusionEnd;

	public static void Conclude()
	{
		Instance._Conclude();
	}

	public static void PayTaxes()
	{
		Instance._PayTaxes();
	}

	public void _PayTaxes()
	{
		BasicCanvas.enabled = true;
		TaxCanvas.enabled = true;

		Time.timeScale = 0;

		var taxRate = GameDataManager.GameValues[GameValueType.TaxRate];
		taxRateText.text	= ((int)(taxRate * 100)).ToString() + "%";
		paidWheat			= Mathf.CeilToInt(GameDataManager.CurrentWheat	* taxRate);
		paidOat				= Mathf.CeilToInt(GameDataManager.CurrentOat	* taxRate);
		forcedPaidWheat		= -Mathf.CeilToInt(GameDataManager.GameValues[GameValueType.ForcedTaxRate] * GameDataManager.CurrentWheat);
		forcedPaidOat		= -Mathf.CeilToInt(GameDataManager.GameValues[GameValueType.ForcedTaxRate] * GameDataManager.CurrentOat);

		StringBuilder sb = new StringBuilder(explanation);
		sb.Replace("{0}", GameDataManager.CurrentWheat.ToString());
		sb.Replace("{1}", GameDataManager.CurrentOat.ToString());
		sb.Replace("{2}", paidWheat.ToString());
		sb.Replace("{3}", paidOat.ToString());

		explanationText.text = sb.ToString();

		wheatSlider.maxValue	= paidWheat;
		wheatSlider.value		= paidWheat;
		oatSlider.maxValue		= paidOat;
		oatSlider.value			= paidOat;
		paidWheatText.text		= paidWheat.ToString();
		paidOatText.text		= paidOat.ToString();
	}

	public void _Conclude()
	{
		if (OnConclusionStart != null)
			OnConclusionStart.Invoke();
		BasicCanvas.enabled = true;
		ConclusionCanvas.enabled = true;

		Time.timeScale = 0;

		// The remain
		if (OnCalculateRemainedCrops != null)
			OnCalculateRemainedCrops.Invoke();
		remainedWheat.text = GameDataManager.CurrentWheat.ToString();
		remainedOat.text = GameDataManager.CurrentOat.ToString();
		
		// Harvested
		if (OnCalculateHarvcetedCrops != null)
			OnCalculateHarvcetedCrops.Invoke();
		var hWheat = 0;
		var hOat = 0;
		if (autoHarvest)
		{
			var wheat	= GameDataManager.CurrentWheat;
			var oat		= GameDataManager.CurrentOat;
			FieldManager.Instance.Harvest();
			hWheat		= GameDataManager.CurrentWheat - wheat;
			hOat		= GameDataManager.CurrentOat - oat;
		}
		ChangeColor(harvestedWheat, hWheat);
		ChangeColor(harvestedOat, hOat);
		harvestedWheat.text = hWheat.ToString();
		harvestedOat.text	= hOat.ToString();

		// Taxes
		if (OnCalculateTaxes != null)
			OnCalculateTaxes.Invoke();
		paidWheat *= -1;
		paidOat *= -1;
		ChangeColor(wheatTax, paidWheat);
		ChangeColor(oatTax, paidOat);
		wheatTax.text				= paidWheat.ToString();
		oatTax.text					= paidOat.ToString();
		GameDataManager.CurrentWheat	+= paidWheat;
		GameDataManager.CurrentOat		+= paidOat;
		GameDataManager.CurrentWheat += forcedPaidWheat;
		GameDataManager.CurrentOat += forcedPaidOat;

		// Family consumption
		if (OnCalculateFamilyConsumption != null)
			OnCalculateFamilyConsumption.Invoke();
		var familyConsumption = 0;
		for (int i = 0; i < FamilyManager.FamilyMembers.Count; i++)
		{
			familyConsumption += Mathf.CeilToInt(FamilyManager.FamilyMembers[i].requiredWheat);
		}
		familyConsumption = 
			Mathf.RoundToInt(
				familyConsumption * 
				GameDataManager.GameValues[GameValueType.WheatConsumptionRate]);
		
		if (familyConsumption > GameDataManager.CurrentWheat)
		{
			GameDataManager.CurrentFamilyHunger += Mathf.CeilToInt((familyConsumption - GameDataManager.CurrentWheat));
			//GameDataManager.GameValues[GameValueType.FamilyHungryDays] ++;
			familyConsumption = GameDataManager.CurrentWheat;
		}
		else
		{
			GameDataManager.CurrentFamilyHunger = 0;
			//GameDataManager.GameValues[GameValueType.FamilyHungryDays] = 0;
		}

		familyConsumption *= -1;
		ChangeColor(tendFamily, familyConsumption);
		tendFamily.text = familyConsumption.ToString();
		GameDataManager.CurrentWheat += familyConsumption;

		// Animal consumption
		if (OnCalculateAnimalConsumption != null)
			OnCalculateAnimalConsumption.Invoke();
		var animalConsumption = 0;
		for (int i = 0; i < GameDatabase.Instance.animalList.Count; i++)
		{
			animalConsumption += Mathf.CeilToInt(GameDatabase.Instance.animalList[i].requiredOat * GameDataManager.GetAnimalNumber(i));
		}
		animalConsumption = 
			Mathf.RoundToInt(
				animalConsumption * 
				GameDataManager.GameValues[GameValueType.OatConsumptionRate]);
		if (animalConsumption > GameDataManager.CurrentOat)
		{
			GameDataManager.CurrentAnimalHunger += Mathf.CeilToInt((animalConsumption - GameDataManager.CurrentOat));
			GameDataManager.GameValues[GameValueType.AnimalHungryDays] ++;
			animalConsumption = GameDataManager.CurrentOat;
		}
		else
		{
			GameDataManager.CurrentAnimalHunger = 0;
			GameDataManager.GameValues[GameValueType.AnimalHungryDays] = 0;
		}
		animalConsumption *= -1;
		ChangeColor(tendAnimal, animalConsumption);
		tendAnimal.text = animalConsumption.ToString();
		GameDataManager.CurrentOat += animalConsumption;

		// Events
		if (OnCalculateEvents != null)
			OnCalculateEvents.Invoke();
		var wDiff = forcedPaidWheat;
		var oDiff = forcedPaidOat;
		ChangeColor(eventWheat, wDiff);
		ChangeColor(eventOat, oDiff);
		eventWheat.text = wDiff.ToString();
		eventOat.text	= oDiff.ToString();

		finalWheat.text = GameDataManager.CurrentWheat.ToString();
		finalOat.text	= GameDataManager.CurrentOat.ToString();

		if (OnConclusionEnd != null)
			OnConclusionEnd.Invoke();
	}

	public void OnPaidWheatChange(float value)
	{
		paidWheat = (int)value;
		paidWheatText.text = paidWheat.ToString();
	}

	public void OnPaidOatChange(float value)
	{
		paidOat = (int)value;
		paidOatText.text = paidOat.ToString();
	}

	public void Close()
	{
		BasicCanvas.enabled = false;
		TaxCanvas.enabled = false;
		ConclusionCanvas.enabled = false;

		Time.timeScale = 1;
	}

	void ChangeColor(Text text, int number)
	{
		if (number > 0)
			text.color = postiveColor;
		else if (number < 0)
			text.color = negativeColor;
		else
			text.color = neutralColor;
	}
}
