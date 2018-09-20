using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
	[SerializeField] float moveSpeed;
	[SerializeField] Vector3 raycastOffset;

	Vector3 target;

	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = GlobalValues.MainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				MoveTowards(hit.point);
			}
		}
	}

	void MoveTowards(Vector3 point)
	{
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
			yield return null;
		}
	}
}
