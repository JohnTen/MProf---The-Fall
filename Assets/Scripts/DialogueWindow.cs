using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;

public class DialogueWindow : MonoSingleton<DialogueWindow>
{
	[SerializeField] Canvas canvas;
	[SerializeField] Image portrait;
	[SerializeField] Text text;

	public void Open(Sprite pic, string text)
	{
		canvas.enabled = true;
		portrait.sprite = pic;
		if (pic == null)
			portrait.color = new Color(0, 0, 0, 0);
		else
		{
			var bound = pic.bounds;
			var ratio = bound.size.x / bound.size.y;
			var size = portrait.rectTransform.sizeDelta;
			size.x = size.y * ratio;
			portrait.rectTransform.sizeDelta = size;
		}

		this.text.text = text;
	}

	public void Close()
	{
		canvas.enabled = false;
	}
}
