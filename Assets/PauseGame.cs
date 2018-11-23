using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
	Canvas canvas;
	bool freeze;

	private void Awake()
	{
		canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape)) return;

		if (freeze)
			Close();
		else
			Open();
	}

	public void Open()
	{
		freeze = true;
		canvas.enabled = true;
		TimeManager.FreezeGameTime();
	}

	public void Close()
	{
		freeze = false;
		canvas.enabled = false;
		TimeManager.UnfreezeGameTime();
	}
}
