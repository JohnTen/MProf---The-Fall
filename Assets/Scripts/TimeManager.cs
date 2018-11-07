using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class TimeManager : GlobalSingleton<TimeManager>
{
	[Tooltip("Current date")]
	[SerializeField] int date;
	[SerializeField] int timeFreezeCount;

	public static event Action<int> OnTimePassed;

	public static int Date
	{
		get { return Instance.date; }
		set { Instance.date = value; }
	}

	public static void FreezeTime()
	{
		Instance.timeFreezeCount++;
		Time.timeScale = 0;
	}

	public static void UnfreezeTime()
	{
		Instance.timeFreezeCount--;
		if (Instance.timeFreezeCount <= 0)
			Time.timeScale = 1;
	}

	public void ToNextTimePeriod()
	{
		date++;
		if (OnTimePassed != null)
			OnTimePassed.Invoke(date);
	}
}
