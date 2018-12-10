using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky : MonoBehaviour
{
	Animator animator;
	public bool Day
	{
		get { return animator.GetBool("Day"); }
		set { animator.SetBool("Day", value); }
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
}
