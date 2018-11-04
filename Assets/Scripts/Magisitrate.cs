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
	Collider collider;

	bool selling;

	private void Awake()
	{
		mover = GetComponent<ObjectMover>();
		animator = GetComponentInChildren<Animator>();
		collider = GetComponentInChildren<Collider>();

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
			collider.enabled = false;
		}
		else if (selling)
		{
			mover.Target = exitPoint;
			mover.OnChangeState += Mover_OnChangeState;
			animator.SetBool("Walk", true);
			collider.enabled = false;
		}
	}

	private void Mover_OnChangeState(ObjectMover mover)
	{
		if (mover.IsMoving) return;

		animator.SetBool("Walk", false);
		collider.enabled = true;
		mover.OnChangeState -= Mover_OnChangeState;
	}
}
