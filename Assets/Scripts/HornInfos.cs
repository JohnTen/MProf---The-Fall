using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class HornInfos : MonoBehaviour
{
	[SerializeField] bool locked;
	[SerializeField] Canvas generalInfoCanvas;
	[SerializeField] Canvas nextWeekCanvas;
	[SerializeField] Canvas shopCanvas;
	[SerializeField] Canvas background;
	[SerializeField] Animator animator;

	public bool IsLocked
	{
		get { return locked; }
	}

	public bool UseBackground
	{
		get { return background.enabled; }
		set { background.enabled = value; }
	}

	public void Close()
	{
		locked = false;
		animator.SetBool("Out", false);
	}

	public void DisplayGeneralInfo()
	{
		if (locked)
		{
			if (shopCanvas.enabled)
				Close();
			if (generalInfoCanvas.enabled)
				Close();
			return;
		}

		locked = true;

		animator.SetBool("Out", true);
		generalInfoCanvas.enabled = true;
		nextWeekCanvas.enabled = false;
		shopCanvas.enabled = false;
	}

	public void DisplayNextWeek()
	{
		if (locked)
		{
			if (shopCanvas.enabled)
				Close();
			if (nextWeekCanvas.enabled)
				Close();
			return;
		}
		locked = true;

		animator.SetBool("Out", true);
		generalInfoCanvas.enabled = false;
		nextWeekCanvas.enabled = true;
		shopCanvas.enabled = false;
	}

	public void DisplayShopInfo()
	{
		if (locked)
		{
			if (shopCanvas.enabled)
				Close();
			return;
		}
		locked = true;

		animator.SetBool("Out", true);
		generalInfoCanvas.enabled = false;
		nextWeekCanvas.enabled = false;
		shopCanvas.enabled = true;
	}
}
