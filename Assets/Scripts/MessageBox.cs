﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class MessageBox : MonoSingleton<MessageBox>
{
	[SerializeField] Canvas canvas;
	[SerializeField] Text titleText;
	[SerializeField] Text contentText;

	[SerializeField] GameObject block;

	public static bool IsShowingMessage { get; private set; }

	Queue<string> contentQueue = new Queue<string>();
	Queue<string> titleQueue = new Queue<string>();

	public static void DisplayMessage(string content)
	{
		DisplayMessage("", content);
	}

	public static void DisplayMessage(string title, string content)
	{
		Instance.block.SetActive(false);
		Instance.block.SetActive(true);
		if (IsShowingMessage)
		{
			Instance.titleQueue.Enqueue(title);
			Instance.contentQueue.Enqueue(content);
			return;
		}

		IsShowingMessage = true;
		Instance.canvas.enabled = true;
		Instance.titleText.text = title;
		Instance.contentText.text = content;
		Time.timeScale = 0;
	}

	public void NextMessage()
	{
		if (contentQueue.Count <= 0)
		{
			IsShowingMessage = false;
			Instance.canvas.enabled = false;
			Time.timeScale = 1;
			return;
		}

		titleText.text = titleQueue.Dequeue();
		contentText.text = contentQueue.Dequeue();
	}
}
