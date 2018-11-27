using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeIn : MonoBehaviour
{
	[SerializeField] float duration = 2;
	[SerializeField] Image image;
	
	void Awake ()
	{
		if (image == null)
			image = GetComponent<Image>();
	}

	public void StartFadein()
	{
		//image.raycastTarget = true;
		StartCoroutine(Fade(1));
	}

	public void StartFadeout()
	{
		//image.raycastTarget = false;
		StartCoroutine(Fade(-1));
	}

	IEnumerator Fade(float amount)
	{
		var timer = duration;

		while (timer > 0)
		{
			timer -= Time.deltaTime;
			var color = image.color;
			color.a += Time.deltaTime * amount;
			image.color = color;
			yield return null;
		}
	}
}
