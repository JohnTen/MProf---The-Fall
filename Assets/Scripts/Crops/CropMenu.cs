using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UnityUtility.Interactables;

public class CropMenu : MonoBehaviour
{
	public event Action<Crop> OnCropChosed;

	public void CloseMenu()
	{
		gameObject.SetActive(false);
		OnCropChosed = null;
	}

	public void OpenMenu()
	{
		gameObject.SetActive(true);
		OnCropChosed = null;
	}

	public void ChooseCrop(int index)
	{
		if (OnCropChosed != null)
		{
			OnCropChosed.Invoke(CropCollection.Instance.cropList[index]);
			OnCropChosed = null;
		}
	}

	private void OnDisable()
	{
		print("Disable");
	}
}
