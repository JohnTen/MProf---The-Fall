using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UnityUtility.Interactables;

public class ButtonMenu : MonoBehaviour
{
	public event Action<int> OnChosed;

	public void CloseMenu()
	{
		gameObject.SetActive(false);
		OnChosed = null;
	}

	public void OpenMenu()
	{
		gameObject.SetActive(true);
		OnChosed = null;
	}

	public void ChooseCrop(int index)
	{
		if (OnChosed != null)
		{
			OnChosed.Invoke(index);
			OnChosed = null;
		}
	}
}
