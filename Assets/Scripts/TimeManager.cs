using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class TimeManager : MonoSingleton<TimeManager>
{
	[Tooltip("How long a day will be in seconds")]
	[SerializeField] float periodDuration;
	
	[Tooltip("Current date")]
	[SerializeField] int date;
	
	[Tooltip("Visualised debuging only")]
	[SerializeField] float timeOfPeriod;

	public static event Action<int> OnTimePassed;

	public static int Date
	{
		get { return Instance.date; }
	}

	public void ToNextTimePeriod()
	{
		date++;
		if (OnTimePassed != null)
			OnTimePassed.Invoke(date);
	}
}
