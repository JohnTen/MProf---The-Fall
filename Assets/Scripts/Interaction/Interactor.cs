using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility.Interactables;

public class Interactor : MonoBehaviour
{
	[SerializeField]
	Transform interactableSign;

	MonoInteractable currentObj;

	// Update is called once per frame
	void Update ()
	{
		CheckInteractable();

		if (currentObj == null)
		{
			interactableSign.gameObject.SetActive(false);
			return;
		}

		interactableSign.gameObject.SetActive(true);
		interactableSign.transform.position = currentObj.transform.position;

		if (Input.GetButtonDown("Fire1"))
		{
			currentObj.StartInteracting();
		}

		if (Input.GetButtonUp("Fire1"))
		{
			currentObj.StopInteracting();
		}
	}

	void CheckInteractable()
	{
		var ray = GlobalValues.MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		Physics.Raycast(ray, out hit);
		var obj = hit.collider.GetComponentInParent<MonoInteractable>();

		if (obj == currentObj) return;
		
		if (obj == null && currentObj.IsInteracting)
		{
			currentObj.StopInteracting();
		}
		currentObj = obj;
	}
}
