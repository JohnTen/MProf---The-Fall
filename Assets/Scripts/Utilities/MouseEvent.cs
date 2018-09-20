using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseEvent : MonoBehaviour
{
	[SerializeField] UnityEvent MouseUp;
	[SerializeField] UnityEvent MouseDown;
	[SerializeField] UnityEvent MouseEnter;
	[SerializeField] UnityEvent MouseOver;
	[SerializeField] UnityEvent MouseExit;

	private void OnMouseUp()
	{
		if (MouseUp != null)
			MouseUp.Invoke();
	}

	private void OnMouseDown()
	{
		if (MouseDown != null)
			MouseDown.Invoke();
	}

	private void OnMouseEnter()
	{
		if (MouseEnter != null)
			MouseEnter.Invoke();
	}

	private void OnMouseOver()
	{
		if (MouseOver != null)
			MouseOver.Invoke();
	}

	private void OnMouseExit()
	{
		if (MouseExit != null)
			MouseExit.Invoke();
	}
}
