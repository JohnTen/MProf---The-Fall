using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityUtility.Interactables;

public class Interactor : MonoBehaviour
{
	[SerializeField] int interactableDistance;
	[SerializeField] Transform interactableSign;

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
		var ray = GameDataManager.MainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		MonoInteractable obj = null;
		
		if (Physics.Raycast(ray, out hit))
		{
			var dist = hit.collider.transform.position - this.transform.position;
			if (dist.sqrMagnitude < interactableDistance * interactableDistance &&
				!EventSystem.current.IsPointerOverGameObject())
				obj = hit.collider.GetComponentInParent<MonoInteractable>();
		}

		if (obj == currentObj) return;
		
		if (obj == null && currentObj != null && currentObj.IsInteracting)
		{
			currentObj.StopInteracting();
		}
		currentObj = obj;
	}
}
