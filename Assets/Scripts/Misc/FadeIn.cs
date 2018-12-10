using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
	[SerializeField] float duration = 2;
	[SerializeField] CanvasGroup canvas;
	
	void Awake ()
	{
		if (canvas == null)
			canvas = GetComponent<CanvasGroup>();

		canvas.interactable = false;
		canvas.blocksRaycasts = false;
		canvas.alpha = 0;
	}

	public void StartFadein()
	{
		canvas.interactable = true;
		canvas.blocksRaycasts = true;
		StartCoroutine(Fade());
	}

	IEnumerator Fade()
	{
		var timer = duration;

		while (timer > 0)
		{
			timer -= Time.deltaTime;
			canvas.alpha = 1 - timer / duration;
			yield return null;
		}
	}
}
