using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class TimeManager : MonoSingleton<TimeManager>
{
	[Tooltip("Current date")]
	[SerializeField] int date;
	[SerializeField] int timeFreezeCount;

	float pausedTime;
	bool paused;

	public static event Action<int> OnTimePassed;

	public static int Date
	{
		get { return Instance.date; }
		set { Instance.date = value; }
	}

	public static float UnscaleDeltaTime
	{
		get
		{
			if (Instance.paused)
				return 0;

			return Time.unscaledDeltaTime;
		}
	}

	public static void FreezeGameTime()
	{
		Instance.timeFreezeCount++;
		Time.timeScale = 0;
		Instance.paused = true;
	}

	public static void UnfreezeGameTime()
	{
		Instance.timeFreezeCount--;
		if (Instance.timeFreezeCount <= 0)
			Time.timeScale = 1;
		Instance.paused = false;
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

	private void OnApplicationFocus(bool focus)
	{
		if (Instance.timeFreezeCount <= 0)
			Time.timeScale = 1;
		paused = !focus;
	}
}
