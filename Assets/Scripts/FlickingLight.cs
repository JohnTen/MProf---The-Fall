using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FlickingLight : MonoBehaviour
{
	[SerializeField] new Light light;
	[SerializeField, MinMaxSlider(0, 3)] Vector2 intensity;
	[SerializeField, MinMaxSlider(0, 0.5f)] Vector2 duration;
	[SerializeField, MinMaxSlider(5, 20f)] Vector2 range;
	[SerializeField, MinMaxSlider(0, 0.2f)] Vector2 xFlickingRange;
	[SerializeField, MinMaxSlider(0, 0.2f)] Vector2 yFlickingRange;
	[SerializeField, MinMaxSlider(0, 0.2f)] Vector2 zFlickingRange;

	Timer timer;
	Vector3 origPos;

	private void Start()
	{
		if (light == null)
			light = GetComponent<Light>();

		timer = new Timer();
		timer.Start(duration.RandomBetween());
		timer.OnTimeOut += Timer_OnTimeOut;

		origPos = transform.position;
	}

	private void Timer_OnTimeOut()
	{
		var duration = this.duration.RandomBetween();
		timer.Start(duration);
		StartCoroutine(Flicking(duration));
	}

	IEnumerator Flicking(float duration)
	{
		var randomValue = Random.value;
		var intensity = this.intensity.LerpBetween(randomValue);
		var range = this.range.LerpBetween(randomValue);

		var curPos = transform.position;
		var newPos = origPos + new Vector3
			(
			xFlickingRange.RandomBetween(),
			yFlickingRange.RandomBetween(),
			zFlickingRange.RandomBetween()
			);

		float time = duration;
		float origIntensity = light.intensity;
		float origRange = light.range;
		while (time > 0)
		{
			time -= Time.deltaTime;
			var t = time / duration;
			print(t);
			light.intensity = Mathf.Lerp(intensity, origIntensity, t);
			light.range = Mathf.Lerp(range, origRange, t);
			transform.position = Vector3.Lerp(newPos, curPos, t);
			yield return null;
		}
	}
}
