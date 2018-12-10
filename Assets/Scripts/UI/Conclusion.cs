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
	[SerializeField] Canvas FamilyFeedCanvas;
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

	[Header("Family Feed")]
	[SerializeField] string remainedFood = @"You have <i>{0}</i> {1} left...";
	[SerializeField] Text remainedFoodText;
	[SerializeField] Text[] NeededFoodTexts;
	[SerializeField] Text[] GaveFoodTexts;
	[SerializeField] Button ContinueButton;

	[Header("Conclusion Texts")]
	[SerializeField] Text remainedWheat;
	[SerializeField] Text remainedOat;
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
	int wheatForFamily;
	int wheatAfterTax;

	int[] requiredFoods;
	int[] gavedFoods;

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
		FamilyFeedCanvas.enabled = false;
		ConclusionCanvas.enabled = false;

		TimeManager.FreezeTime();

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

	public void _FeedFamily()
	{
		var list = FamilyManager.FamilyMembers;
		var neededFood = 0;

		wheatAfterTax = GameDataManager.CurrentWheat - paidWheat - forcedPaidWheat;

		foreach (var f in list)
		{
			neededFood += f.requiredWheat;
		}

		//if (neededFood <= GameDataManager.CurrentWheat)
		//{
		//	wheatForFamily = neededFood;
		//	ContinueButton.onClick.Invoke();
		//	return;
		//}

		BasicCanvas.enabled = true;
		TaxCanvas.enabled = false;
		FamilyFeedCanvas.enabled = true;
		ConclusionCanvas.enabled = false;

		wheatForFamily = 0;
		var totalFood = wheatAfterTax;
		for (int i = 0; i < list.Count; i ++)
		{
			requiredFoods[i] = list[i].gone? 0 : list[i].hunger + list[i].requiredWheat;

			if (requiredFoods[i] <= totalFood)
			{
				gavedFoods[i] = requiredFoods[i];
			}
			else
			{
				gavedFoods[i] = totalFood;
			}

			totalFood -= gavedFoods[i];
			wheatForFamily += gavedFoods[i];
		}

		UpdateFamilyFeedTexts();
	}

	void UpdateFamilyFeedTexts()
	{
		var remainedFood = wheatAfterTax - wheatForFamily;

		StringBuilder sb = new StringBuilder(this.remainedFood);
		sb.Replace("{0}", remainedFood > 0 ? remainedFood.ToString() : "no");
		sb.Replace("{1}", remainedFood > 1? "foods": "food");

		remainedFoodText.text = sb.ToString();

		for (int i = 0; i < requiredFoods.Length; i ++)
		{
			NeededFoodTexts[i].text = requiredFoods[i].ToString();
			GaveFoodTexts[i].text = gavedFoods[i].ToString();
		}
	}

	public void RaiseFeed(int index)
	{
		if (gavedFoods[index] < requiredFoods[index] && wheatAfterTax > wheatForFamily)
		{
			gavedFoods[index]++;
			wheatForFamily++;
			UpdateFamilyFeedTexts();
		}
	}

	public void LowerFeed(int index)
	{
		if (gavedFoods[index] > 0)
		{
			gavedFoods[index]--;
			wheatForFamily--;
			UpdateFamilyFeedTexts();
		}
	}

	public void _Conclude()
	{
		BasicCanvas.enabled = true;
		TaxCanvas.enabled = false;
		FamilyFeedCanvas.enabled = false;
		ConclusionCanvas.enabled = true;

		TimeManager.FreezeTime();

		// The remain
		remainedWheat.text = GameDataManager.CurrentWheat.ToString();
		remainedOat.text = GameDataManager.CurrentOat.ToString();

		// Taxes
		var wholeTax = wheatSlider.maxValue + oatSlider.maxValue;
		var paidTax = paidWheat + paidOat;
		if (wholeTax * 0.5f > paidTax)
			EventManager.Instance.taxEvasionTimes++;

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
		var list = FamilyManager.FamilyMembers;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].hunger = requiredFoods[i] - gavedFoods[i];
		}

		wheatForFamily *= -1;
		ChangeColor(tendFamily, wheatForFamily);
		tendFamily.text = wheatForFamily.ToString();
		GameDataManager.CurrentWheat += wheatForFamily;

		// Animal consumption
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

		// Milk
		GameDataManager.GameValues[GameValueType.Milks] += GameDataManager.GameValues[GameValueType.NumberOfOx] * GameDatabase.Instance.animalList[0].ProvidedFood;

		// Events
		var wDiff = forcedPaidWheat;
		var oDiff = forcedPaidOat;
		ChangeColor(eventWheat, wDiff);
		ChangeColor(eventOat, oDiff);
		eventWheat.text = wDiff.ToString();
		eventOat.text	= oDiff.ToString();

		finalWheat.text = GameDataManager.CurrentWheat.ToString();
		finalOat.text	= GameDataManager.CurrentOat.ToString();
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
		FamilyFeedCanvas.enabled = false;
		ConclusionCanvas.enabled = false;

		TimeManager.UnfreezeTime();
		TimeManager.UnfreezeTime();
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

	protected override void Awake()
	{
		base.Awake();

		var list = FamilyManager.FamilyMembers;
		requiredFoods = new int[list.Count];
		gavedFoods = new int[list.Count];
	}
}
