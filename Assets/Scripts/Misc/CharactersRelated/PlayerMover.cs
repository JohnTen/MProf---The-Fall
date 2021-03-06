﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Refactor, separate click and move
public class PlayerMover : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] Vector3 raycastOffset;
	[SerializeField] SpriteRenderer pcRender;
	[SerializeField] string walkingSoundLabel = "Walking";

	Animator playerAnimator;

	Vector3 target;

	private void Awake()
	{
		playerAnimator = GetComponent<Animator>();
		pcRender = GetComponentInChildren<SpriteRenderer>();
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			var ray = GameDataManager.MainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			var layer = Physics.DefaultRaycastLayers & ~(1 << 10);

			if (Physics.Raycast(ray, out hit, 40, layer))
			{
				MoveTowards(hit.point);
			}
		}
	}

	void MoveTowards(Vector3 point)
	{
		// TODO: move this to a proper place
		WorldUI.CloseAllMenu();
		point.y = transform.position.y;
		target = point;

		StopAllCoroutines();
		StartCoroutine(Moving());
	}

	void StopMoving()
	{
		StopAllCoroutines();
	}

	IEnumerator Moving()
	{
		playerAnimator.SetBool("Walking", true);
		while (true)
		{
			var dir = target - transform.position;
			var moveDist = moveSpeed * Time.deltaTime;
			if (moveDist * moveDist > dir.sqrMagnitude)
			{
				transform.position = target;
			}

			if (transform.position == target) break;

			if (Physics.Raycast(transform.position + raycastOffset, dir, moveDist))
			{
				break;
			}

			transform.Translate(dir.normalized * moveDist);
			
			pcRender.flipX = dir.x > 0;

			if (!SoundManager.IsPlaying(walkingSoundLabel) && Time.timeScale != 0)
				SoundManager.Play(walkingSoundLabel);

			yield return null;
		}
		playerAnimator.SetBool("Walking", false);
	}
}
