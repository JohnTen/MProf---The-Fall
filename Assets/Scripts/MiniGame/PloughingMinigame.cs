﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;


public class PloughingMinigame : BaseMinigame
{
	[SerializeField, MinMaxSlider(0, 8)] Vector2Int hitSignNumber = new Vector2Int(4,4);
	[SerializeField, MinMaxSlider(0, 1)] Vector2 hitSignRange = new Vector2(0.2f, 0.8f);
	[SerializeField] float minSignDistance = 50;
	[SerializeField] float pointerMoveSpeed = 10;
	[SerializeField] float hitZoneWidth = 50;
	[SerializeField] GameObject hitZonePrefab;
	[SerializeField] GameObject hitSignPrefab;
	[SerializeField] RectTransform plow;
	[SerializeField] RectTransform plowMask;
	[SerializeField] RectTransform ground;
	[SerializeField] Canvas canvas;
	[SerializeField] Color initialHitZoneColor = new Color(0, 1, 1, 0.2f);
	[SerializeField] Color failedHitZoneColor = new Color(1, 0, 0, 0.2f);
	[SerializeField] Color successedHitZoneColor = new Color(0, 1, 0, 0.2f);

	[Header("Debug")]
	[SerializeField] int lastHitZone;
	[SerializeField] List<RectTransform> hitZones = new List<RectTransform>();
	[SerializeField] List<RectTransform> hitSigns = new List<RectTransform>();
	[SerializeField] List<RectTransform> finalZones = new List<RectTransform>();
	[SerializeField] List<bool> hitZoneSuccess = new List<bool>();

	[SerializeField] List<KeyCode> hitCodes = new List<KeyCode>();

	public override bool IsPlaying { get; protected set; }

	public override event Action<float> OnGameFinished;

	public override void StartPlay(int choice)
	{
		IsPlaying = true;
		canvas.enabled = true;
		OnGameFinished = null;

		TimeManager.FreezeTime();

		plow.localPosition -= Vector3.right * plow.localPosition.x;
		plowMask.sizeDelta = Vector2.zero;
		lastHitZone = -1;

		Randomise();

		plowMask.SetAsFirstSibling();
		plow.SetAsFirstSibling();
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
		var createdSignNumber = hitSignNumber.RandomBetweenIncluded();

		// Ensure we have enough hitzones and signs to play around
		while (hitZones.Count < createdSignNumber)
		{
			var newSign = Instantiate(hitSignPrefab).transform as RectTransform;
			var newZone = Instantiate(hitZonePrefab).transform as RectTransform;

			newZone.SetParent(ground);
			newSign.SetParent(ground);

			newSign.localScale = Vector3.one;
			newZone.localScale = Vector3.one;

			newZone.SetAsFirstSibling();
			newSign.SetAsLastSibling();

			hitSigns.Add(newSign);
			hitZones.Add(newZone);
		}
		
		// Enable the zones/signs we needed and disable the rest
		for (int i = 0; i < hitZones.Count; i ++)
		{
			if (i < createdSignNumber)
			{
				hitZones[i].gameObject.SetActive(true);
				hitSigns[i].gameObject.SetActive(true);
			}
			else
			{
				hitZones[i].gameObject.SetActive(false);
				hitSigns[i].gameObject.SetActive(false);
			}
		}

		hitCodes.Clear();
		finalZones.Clear();
		hitZoneSuccess.Clear();
		var positionOffset = hitSignRange.x * ground.rect.width;
		// Reposition each zone/sigh
		for (int i = 0; i < createdSignNumber; i++)
		{
			var isAbleToGetPosition = true;
			// Try to get a new position
			var randomX = Random.Range(ground.rect.xMin, ground.rect.xMax);
			randomX = randomX * (hitSignRange.y - hitSignRange.x) + positionOffset;

			// Verify the position
			int triedTimes = 0;
			for (int j = 0; j < finalZones.Count; j++)
			{
				if (IsWithinMinDistance(finalZones[j], randomX))
				{
					randomX = Random.Range(ground.rect.xMin, ground.rect.xMax);
					randomX = randomX * (hitSignRange.y - hitSignRange.x) + positionOffset;
					j = -1;
					triedTimes++;
				}

				if (triedTimes > 100)
				{
					isAbleToGetPosition = false;
					break;
				}
			}

			// If cannot find space for new hit signs, disable the rest and be done with the setup
			if (!isAbleToGetPosition)
			{
				for (int j = i; j < finalZones.Count; j++)
				{
					hitZones[j].gameObject.SetActive(false);
					hitSigns[j].gameObject.SetActive(false);
				}
				break;
			}

			// Set the new position for hitzone/hitsign
			finalZones.Add(hitZones[i]);
			hitSigns[i].localPosition = new Vector3(randomX, 0);
			hitZones[i].localPosition = new Vector3(randomX, 0);
			finalZones[i].sizeDelta = new Vector2(finalZones[i].sizeDelta.x, 0);
			finalZones[i].GetComponent<Image>().color = initialHitZoneColor;
			hitZoneSuccess.Add(false);

			var randomHitCode = Random.Range(0, 4);

			switch (randomHitCode)
			{
				case 0: hitCodes.Add(KeyCode.A); break;
				case 1: hitCodes.Add(KeyCode.S); break;
				case 2: hitCodes.Add(KeyCode.D); break;
				case 3: hitCodes.Add(KeyCode.F); break;
			}

			hitSigns[i].GetComponentInChildren<Text>().text = hitCodes[i].ToString();
		}
	}

	private bool IsWithinMinDistance(RectTransform zone, float position)
	{
		var zonePos = zone.localPosition.x;

		return 
			(position < zonePos && position + minSignDistance >= zonePos) || 
			(position > zonePos && position - minSignDistance <= zonePos);
	}

	private bool IsWithinZone(RectTransform zone, float pos)
	{
		Vector2 point = new Vector2(pos-zone.localPosition.x, zone.localPosition.y);
		return zone.rect.Contains(point);
	}

	private void Update()
	{
		if (!IsPlaying) return;

		plow.transform.localPosition += Vector3.right * pointerMoveSpeed * Time.unscaledDeltaTime;
		plowMask.sizeDelta += Vector2.right * pointerMoveSpeed * Time.unscaledDeltaTime;

		if (plow.transform.localPosition.x >= ground.rect.xMax)
		{
			float successCount = 0;
			for (int i = 0; i < hitZoneSuccess.Count; i++)
			{
				if (hitZoneSuccess[i])
					successCount++;
			}
			float successRate = successCount / hitZoneSuccess.Count;
			
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;

			if (successRate <= 0)
				MessageBox.DisplayMessage("Minigame Failed!", "You failed to plouge the field!");
			else if (successRate < 0.8f)
				MessageBox.DisplayMessage("Minigame Passed!", "You plouged the field but you could do better!");
			else
				MessageBox.DisplayMessage("Minigame Successed!", "You plouged the field perfectly!");

			if (OnGameFinished != null)
				OnGameFinished(successRate);

			return;
		}

		if (lastHitZone >= 0)
		{
			var image = finalZones[lastHitZone].GetComponent<Image>();
			if (IsWithinZone(finalZones[lastHitZone], plow.localPosition.x))
			{
				if (Input.GetKeyDown(hitCodes[lastHitZone]))
				{
					hitZoneSuccess[lastHitZone] = true;
					image.color = successedHitZoneColor;
					lastHitZone = -1;
				}
			}
			else
			{
				if (!hitZoneSuccess[lastHitZone])
				{
					image.color = failedHitZoneColor;
				}
				lastHitZone = -1;
			}
		}
		else
		{
			for (int i = 0; i < finalZones.Count; i++)
			{
				if (IsWithinZone(finalZones[i], plow.localPosition.x) && !hitZoneSuccess[i])
				{
					lastHitZone = i;
					break;
				}
			}
		}
	}
}