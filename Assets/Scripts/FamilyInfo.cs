using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FamilyInfo : MonoBehaviour
{
	[SerializeField] float rollingSpeed;
	[SerializeField] float openedHeight;
	[SerializeField] float closedHeight;
	[SerializeField] RectTransform infoPanel;
	[SerializeField] Text nameText;
	[SerializeField] Text hungerText;
	[SerializeField] Text sanityText;
	[SerializeField] Text dRateText;
	[SerializeField] Button[] buttons;
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < FamilyManager.FamilyMembers.Count; i++)
		{
			buttons[i].interactable = !FamilyManager.FamilyMembers[i].died;
		}

		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			Close();
		}
	}

	public void Display(int i)
	{
		Open();
		var list = FamilyManager.FamilyMembers;
		nameText.text = list[i].name;
		hungerText.text = list[i].hunger.ToString();
		sanityText.text = list[i].mentalHealth.ToString();
		dRateText.text = (list[i].dyingRate + list[i].mentalDyingRatio * list[i].mentalHealth).ToString();
	}

	void Open()
	{
		var size = infoPanel.sizeDelta;
		size.y = openedHeight;
		infoPanel.sizeDelta = size;
	}

	void Close()
	{
		var size = infoPanel.sizeDelta;
		size.y = closedHeight;
		infoPanel.sizeDelta = size;
	}
}
