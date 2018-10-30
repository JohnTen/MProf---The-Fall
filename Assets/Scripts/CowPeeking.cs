using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowPeeking : MonoBehaviour
{
	Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnMouseDown()
	{
		animator.SetBool("Peek", true);
		Invoke("DisablePeek", 1);
	}

	private void DisablePeek()
	{
		animator.SetBool("Peek", false);
	}
}
