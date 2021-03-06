﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CowPeeking : MonoBehaviour
{
    bool mooing;
	Animator animator;
    [SerializeField] UnityEvent OnClick;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void OnMouseDown()
	{
        if (mooing || EventSystem.current.IsPointerOverGameObject()) return;
        mooing = true;
        OnClick.Invoke();
        animator.SetBool("Peek", true);
		Invoke("DisablePeek", 1);
	}

	private void DisablePeek()
	{
        mooing = false;

        animator.SetBool("Peek", false);
	}
}
