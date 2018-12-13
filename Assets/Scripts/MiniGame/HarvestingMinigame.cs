using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class HarvestingMinigame : BaseMinigame
{
	[SerializeField] RectTransform meter;
	[SerializeField] RectTransform pointer;
	[SerializeField] RectTransform hitZone1;
	[SerializeField] RectTransform hitZone2;
	[SerializeField] Animator sickleAnimator;
	[SerializeField] Sprite[] cropSprites;

	[SerializeField] Vector2 hitZone1Size;
	[SerializeField] Vector2 hitZone2Size;
	public float PointerSpeed;

	[Header("Debug")]
	bool playing;
	bool movingDir;
	float pointerMovingRange;
	[SerializeField] Canvas canvas;
	[SerializeField] float animationDelay = 0.8f;

	public override bool IsPlaying
	{
		get { return playing; }
		protected set { playing = value; }
	}

	public override event Action<float> OnGameFinished;

	public override void StartPlay(int choice)
	{
		IsPlaying = true;
		canvas.enabled = true;
		OnGameFinished = null;

		Randomise();

		meter.GetComponent<Image>().sprite = cropSprites[choice];

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

		sickleAnimator.SetBool("Slice", false);

		TimeManager.UnfreezeTime();
		MessageBox.OnContinue -= StopPlay;
	}

	private void Randomise()
	{
		var height1 = Random.Range(hitZone1Size.x, hitZone1Size.y);
		var height2 = Random.Range(hitZone2Size.x, hitZone2Size.y);

		var size = hitZone1.sizeDelta;
		size.y = height1;
		hitZone1.sizeDelta = size;

		size = hitZone2.sizeDelta;
		size.y = height2;
		hitZone2.sizeDelta = size;

		var maxY = (meter.sizeDelta.y - height2) / 2;
		var minY = -maxY;

		var yPos = Random.Range(minY, maxY);
		var pos = hitZone1.localPosition;
		pos.y = yPos;
		hitZone1.localPosition = pos;
		hitZone2.localPosition = pos;
	}

	private void AfterSlice()
	{
		var maxHitZone1 = hitZone1.localPosition.y + hitZone1.sizeDelta.y * 0.5f;
		var minHitZone1 = hitZone1.localPosition.y - hitZone1.sizeDelta.y * 0.5f;
		var maxHitZone2 = hitZone2.localPosition.y + hitZone2.sizeDelta.y * 0.5f;
		var minHitZone2 = hitZone2.localPosition.y - hitZone2.sizeDelta.y * 0.5f;

		MessageBox.OnContinue += StopPlay;
		if (pointer.localPosition.y > minHitZone1 &&
			pointer.localPosition.y < maxHitZone1)
		{
			MessageBox.DisplayMessage("Minigame won!", "You successfully harvest the crop!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(1);
			}
		}
		else if (
			pointer.localPosition.y > minHitZone2 &&
			pointer.localPosition.y < maxHitZone2)
		{
			MessageBox.DisplayMessage("Minigame Passed!", "A quarter of the crops you harvested are lost!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(0.5f);
			}
		}
		else
		{
			MessageBox.DisplayMessage("Minigame Failed!", "Half of the crops you harvested are lost!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(0);
			}
		}
	}

	private void Start()
	{
		if (canvas == null)
			canvas = GetComponent<Canvas>();
	}

	private void Update()
	{
		if (!IsPlaying) return;

		var dir = Vector3.up * TimeManager.UnscaleDeltaTime * PointerSpeed;
		dir = movingDir ? -dir : dir;

		pointer.localPosition += dir;
		if (Mathf.Abs(pointer.localPosition.y) > pointerMovingRange)
		{
			movingDir = !movingDir;
		}

		if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space))
		{
			IsPlaying = false;
			StartCoroutine(StartAnimation());
		}

		return;
		if (Input.GetKeyDown(KeyCode.O))
		{
			MessageBox.DisplayMessage("Minigame won!", "You successfully harvest the crop!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(1);
			}
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			MessageBox.DisplayMessage("Minigame failed!", "Half of the crops you harvested are lost!");
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(1);
			}
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
		}
	}

	IEnumerator StartAnimation()
	{
		sickleAnimator.SetBool("Slice", true);

		float timer = 0;
		while (timer < animationDelay)
		{
			timer += Time.unscaledDeltaTime;
			sickleAnimator.Update(Time.unscaledDeltaTime);
			yield return null;
		}
		AfterSlice();
	}
}
