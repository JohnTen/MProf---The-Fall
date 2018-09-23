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

	[SerializeField] float taxRate = 0.33f;
	
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

		taxRateText.text = ((int)(taxRate * 100)).ToString() + "%";
		paidWheat	= Mathf.CeilToInt(GameDataManager.CurrentWheat * taxRate);
		paidOat		= Mathf.CeilToInt(GameDataManager.CurrentOat * taxRate);

		StringBuilder sb = new StringBuilder(explanation);
		sb.Replace("{0}", GameDataManager.OffsetedWheat.ToString());
		sb.Replace("{1}", GameDataManager.OffsetedOat.ToString());
		sb.Replace("{2}", paidWheat.ToString());
		sb.Replace("{3}", paidOat.ToString());

		explanationText.text = sb.ToString();

		wheatSlider.maxValue	= paidWheat;
		wheatSlider.value		= paidWheat;
		oatSlider.maxValue		= paidOat;
		oatSlider.value			= paidOat;
	}

	public void _Conclude()
	{
		BasicCanvas.enabled = true;
		ConclusionCanvas.enabled = true;
		remainedWheat.text = GameDataManager.CurrentWheat.ToString();
		remainedOat.text = GameDataManager.CurrentOat.ToString();

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

		paidWheat *= -1;
		paidOat *= -1;
		ChangeColor(wheatTax, paidWheat);
		ChangeColor(oatTax, paidOat);
		wheatTax.text				= paidWheat.ToString();
		oatTax.text					= paidOat.ToString();
		GameDataManager.CurrentWheat	+= paidWheat;
		GameDataManager.CurrentOat		+= paidOat;

		var tFamily = -GameDataManager.CurrentFamily;
		if (GameDataManager.CurrentFamily > GameDataManager.OffsetedWheat)
		{
			tFamily = -GameDataManager.OffsetedWheat;
			GameDataManager.CurrentFamilyHunger += GameDataManager.CurrentFamily - GameDataManager.OffsetedWheat;
		}
		ChangeColor(tendFamily, tFamily);
		tendFamily.text = tFamily.ToString();
		GameDataManager.CurrentWheat += tFamily;

		var tAnimal = 0;
		for (int i = 0; i < GameDataManager.CurrentAnimals.Length; i++)
		{
			tAnimal += DataBase.Instance.animalList[i].requiredOat * GameDataManager.CurrentAnimals[i];
		}
		if (tAnimal > GameDataManager.OffsetedOat)
		{
			GameDataManager.CurrentAnimalHunger += tAnimal - GameDataManager.OffsetedOat;
			tAnimal = GameDataManager.OffsetedOat;
		}
		tAnimal *= -1;
		ChangeColor(tendAnimal, tAnimal);
		tendAnimal.text = tAnimal.ToString();
		GameDataManager.CurrentOat += tAnimal;

		ChangeColor(eventWheat, GameDataManager.EventOffset.crops[0]);
		ChangeColor(eventOat, GameDataManager.EventOffset.crops[1]);
		eventWheat.text = GameDataManager.EventOffset.crops[0].ToString();
		eventOat.text	= GameDataManager.EventOffset.crops[1].ToString();
		GameDataManager.CurrentWheat	+= GameDataManager.EventOffset.crops[0];
		GameDataManager.CurrentOat		+= GameDataManager.EventOffset.crops[1];

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
		ConclusionCanvas.enabled = false;
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
