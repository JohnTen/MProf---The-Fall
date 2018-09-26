using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class TimeManager : GlobalSingleton<TimeManager>
{
	[Tooltip("Current date")]
	[SerializeField] int date;

	[SerializeField] GameEvent eve;

	public static event Action<int> OnTimePassed;

	public static int Date
	{
		get { return Instance.date; }
		set { Instance.date = value; }
	}

	public void ToNextTimePeriod()
	{
		date++;
		if (OnTimePassed != null)
			OnTimePassed.Invoke(date);

		//eve.CheckEvent();
	}
}
