using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class HarvestingMinigame : BaseMinigame
{
	[SerializeField] RectTransform meter;
	[SerializeField] RectTransform pointer;
	[SerializeField] RectTransform hitZone;

	public float MinHitZoneSize;
	public float MaxHitZoneSize;
	public float PointerSpeed;

	[Header("Debug")]
	bool playing;
	bool movingDir;
	float pointerMovingRange;
	[SerializeField] Canvas canvas;

	public override bool IsPlaying
	{
		get { return playing; }
		protected set { playing = value; }
	}

	public override event Action<bool> OnGameFinished;

	public override void StartPlay(int choice)
	{
		IsPlaying = true;
		canvas.enabled = true;
		OnGameFinished = null;

		Randomise();

		pointerMovingRange = (meter.sizeDelta.y - pointer.sizeDelta.y) / 2;

		var pos = pointer.localPosition;
		pos.y = -pointerMovingRange;
		pointer.localPosition = pos;
		movingDir = false;

		TimeManager.FreezeTime();
	}

	public override void StopPlay()
	{
		IsPlaying = false;
		canvas.enabled = false;

		TimeManager.UnfreezeTime();
		MessageBox.OnContinue -= StopPlay;
	}

	private void Randomise()
	{
		var height = Random.Range(MinHitZoneSize, MaxHitZoneSize);

		var size = hitZone.sizeDelta;
		size.y = height;
		hitZone.sizeDelta = size;

		var maxY = (meter.sizeDelta.y - height) / 2;
		var minY = -maxY;

		var yPos = Random.Range(minY, maxY);
		var pos = hitZone.localPosition;
		pos.y = yPos;
		hitZone.localPosition = pos;
	}

	private void Start()
	{
		if (canvas == null)
			canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
		if (!IsPlaying) return;

		var dir = Vector3.up * Time.unscaledDeltaTime * PointerSpeed;
		dir = movingDir ? -dir : dir;

		pointer.localPosition += dir;
		if (Mathf.Abs(pointer.localPosition.y) > pointerMovingRange)
		{
			movingDir = !movingDir;
		}

		if (Input.GetButtonDown("Fire1"))
		{
			var maxHitZone = hitZone.localPosition.y + hitZone.sizeDelta.y * 0.5f;
			var minHitZone = hitZone.localPosition.y - hitZone.sizeDelta.y * 0.5f;

			if (pointer.localPosition.y > minHitZone &&
				pointer.localPosition.y < maxHitZone)
			{
				MessageBox.DisplayMessage("Minigame won!", "You successfully harvest the crop!");
				if (OnGameFinished != null)
				{
					OnGameFinished.Invoke(true);
				}
			}
			else
			{
				MessageBox.DisplayMessage("Minigame failed!", "Half of the crops you harvested are lost!");
				if (OnGameFinished != null)
				{
					OnGameFinished.Invoke(false);
				}
			}

			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			MessageBox.DisplayMessage("Minigame won!", "You successfully harvest the crop!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(true);
			}
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			MessageBox.DisplayMessage("Minigame failed!", "Half of the crops you harvested are lost!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(true);
			}
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
		}
	}
}
