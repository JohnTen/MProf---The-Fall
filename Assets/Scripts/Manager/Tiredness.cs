using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class Tiredness : MonoBehaviour
{
	[SerializeField] RectTransform clockHandPivot;
	[SerializeField] float time;
	[SerializeField] UnityEvent OnTirednessReached;
	[SerializeField] UnityEvent HalfWayThrough;
	[SerializeField] UnityEvent NewDay;

	Timer timer;
	bool halfThrough = false;

	// Use this for initialization
	void Start ()
	{
		timer = new Timer();
		timer.OnTimeOut += OnTirednessReached.Invoke;
		StartTiming();
	}

	private void Update()
	{
		clockHandPivot.rotation = Quaternion.AngleAxis(Mathf.Lerp(0, 360, timer.PassedPercentage), Vector3.forward);
		if (!halfThrough && timer.PassedPercentage < 0.5f)
		{
			halfThrough = true;
			HalfWayThrough.Invoke();
		}
	}

	private void OnDestroy()
	{
		timer.Dispose();
	}

	public void StartTiming()
	{
        if (halfThrough)
        {
			halfThrough = false;
			NewDay.Invoke();
        }
        timer.Start(time);
    }

	public void StopTiming()
	{
		timer.Abort();
	}
}
