using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUpdater : MonoBehaviour
{
	[SerializeField] Text FoodText;
	[SerializeField] Text DateText;

	private void Awake()
	{
		GlobalValues.OnFoodChanged += GlobalValues_OnFoodChanged;
		TimeManager.OnTimePassed += TimeManager_OnTimePassed;

		DateText.text = TimeManager.Date.ToString();
		FoodText.text = GlobalValues.CurrentFood.ToString();
	}

	private void TimeManager_OnTimePassed(int date)
	{
		DateText.text = date.ToString();
	}

	private void GlobalValues_OnFoodChanged(int food)
	{
		FoodText.text = food.ToString();
	}
}
