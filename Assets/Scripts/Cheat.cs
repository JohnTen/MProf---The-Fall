using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cheat : MonoBehaviour
{
	[SerializeField] InputField input;

	public void ChangeWheat()
	{
		GameDataManager.GameValues[GameValueType.NumberOfWheat] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
	public void ChangeOat()
	{
		GameDataManager.GameValues[GameValueType.NumberOfOat] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
	
	public void ChangeOx()
	{
		GameDataManager.GameValues[GameValueType.NumberOfOx] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
	public void ChangeWife()
	{
		GameDataManager.GameValues[GameValueType.NumberOfWife] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
	public void ChangeSon()
	{
		GameDataManager.GameValues[GameValueType.NumberOfSon] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
	public void ChangeDaugther()
	{
		GameDataManager.GameValues[GameValueType.NumberOfDaughter] += int.Parse(input.text);
		GameDataManager.UpdateValues();
	}
}
