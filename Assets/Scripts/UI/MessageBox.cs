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
	[SerializeField] Canvas continueButton;
	[SerializeField] Canvas yesButton;
	[SerializeField] Canvas noButton;

	[SerializeField] GameObject block;

	Queue<string> contentQueue = new Queue<string>();
	Queue<string> titleQueue = new Queue<string>();
	Queue<Action> yesQueue = new Queue<Action>();
	Queue<Action> noQueue = new Queue<Action>();

	public static bool IsShowingMessage { get { return Instance.isShowingMessage; } }
	public static event Action OnContinue
	{
		add { Instance.onContinue += value; }
		remove { Instance.onContinue -= value; }
	}

	public static event Action OnFinished
	{
		add { Instance.onFinished += value; }
		remove { Instance.onFinished -= value; }
	}

	bool isShowingMessage;
	Action onContinue;
	Action onFinished;
	Action yesAction;
	Action noAction;

	public static void DisplayMessage(string content)
	{
		DisplayMessage("", content, null, null);
	}

	public static void DisplayMessage(string title, string content)
	{
		DisplayMessage(title, content, null, null);
	}

	public static void DisplayMessage(string title, string content, Action onYes, Action onNo)
	{
		if (IsShowingMessage)
		{
			Instance.titleQueue.Enqueue(title);
			Instance.contentQueue.Enqueue(content);
			Instance.yesQueue.Enqueue(onYes);
			Instance.noQueue.Enqueue(onNo);
			return;
		}

		Instance.isShowingMessage = true;
		Instance.block.SetActive(false);
		Instance.canvas.enabled = true;
		Instance.titleText.text = title;
		Instance.contentText.text = content;
		Instance.yesAction = onYes;
		Instance.noAction = onNo;
		Instance.block.SetActive(true);
		Canvas.ForceUpdateCanvases();

		if (onYes == null && onNo == null)
		{
			Instance.continueButton.enabled = true;
			Instance.yesButton.enabled = false;
			Instance.noButton.enabled = false;
		}
		else
		{
			Instance.continueButton.enabled = false;
			Instance.yesButton.enabled = true;
			Instance.noButton.enabled = true;
		}
		Canvas.ForceUpdateCanvases();

		TimeManager.FreezeTime();
	}

	public void Choice(bool yes)
	{
		if (yes)
		{
			if (yesAction != null)
				yesAction.Invoke();
		}
		else
		{
			if (noAction != null)
				noAction.Invoke();
		}
		NextMessage();
	}

	public void NextMessage()
	{
		if (onContinue != null)
			onContinue.Invoke();

		if (contentQueue.Count <= 0)
		{
			isShowingMessage = false;
			Instance.canvas.enabled = false;
			TimeManager.UnfreezeTime();

			if (onFinished != null)
				onFinished.Invoke();
			return;
		}

		titleText.text = titleQueue.Dequeue();
		contentText.text = contentQueue.Dequeue();
		yesAction = yesQueue.Dequeue();
		noAction = noQueue.Dequeue();
	}
}
