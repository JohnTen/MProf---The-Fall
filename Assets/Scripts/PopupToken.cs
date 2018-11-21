using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupToken : MonoBehaviour
{
	[SerializeField] float duration = 2;
	[SerializeField] float xMulti = 0;
	[SerializeField] float yMulti = 10;
	[SerializeField] AnimationCurve xMovement;
	[SerializeField] AnimationCurve yMovement;
	[SerializeField] Gradient color;

	Text text;
	float timer = 0;
	Vector3 startPosition;

	public Gradient Color
	{
		get { return color; }
		set { color = value; }
	}

	public Text Text
	{
		get
		{
			if (text != null)
				return text;

			text = GetComponent<Text>();
			return text;
		}
	}

	private void Start()
	{
		startPosition = transform.localPosition;
	}

	void Update ()
	{
		if (timer >= duration)
		{
			Destroy(this.gameObject);
			return;
		}

		timer += Time.deltaTime;

		Text.color = color.Evaluate(timer / duration);
		transform.localPosition = 
			startPosition + 
			new Vector3(
				xMovement.Evaluate(timer / duration) * xMulti, 
				yMovement.Evaluate(timer / duration) * yMulti
			);
	}
}
