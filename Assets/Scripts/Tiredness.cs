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
	
	Timer timer;

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
	}

	public void StartTiming()
	{
		timer.Start(time);
	}
}
