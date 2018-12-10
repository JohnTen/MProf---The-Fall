using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility;

public class Magisitrate : MonoBehaviour
{
	[SerializeField] int showupDuration;
	[SerializeField] Transform enterPoint;
	[SerializeField] Transform standPoint;
	[SerializeField] Transform exitPoint;

	ObjectMover mover;
	Animator animator;
	Collider ccollider;

	bool selling;

	private void Start()
	{
		mover = GetComponent<ObjectMover>();
		animator = GetComponentInChildren<Animator>();
		ccollider = GetComponentInChildren<Collider>();

		TimeManager.OnTimePassed += TimeManager_OnTimePassed;
	}

	private void TimeManager_OnTimePassed(int date)
	{
		if (date % showupDuration == 0)
		{
			selling = true;
			transform.position = enterPoint.position;
			mover.Target = standPoint;
			mover.OnChangeState += Mover_OnChangeState;
			animator.SetBool("Walk", true);
			ccollider.enabled = false;
		}
		else if (selling)
		{
			mover.Target = exitPoint;
			mover.OnChangeState += Mover_OnChangeState;
			animator.SetBool("Walk", true);
			ccollider.enabled = false;
		}
	}

	private void Mover_OnChangeState(ObjectMover mover)
	{
		if (mover.IsMoving) return;

		animator.SetBool("Walk", false);
		ccollider.enabled = true;
		mover.OnChangeState -= Mover_OnChangeState;
	}
}
