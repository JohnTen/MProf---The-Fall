using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessEffect : MonoBehaviour
{
	[SerializeField] PostProcessingProfile profile;
	ColorGradingModel.Settings origSetting;

	private void Awake()
	{
		origSetting = profile.colorGrading.settings;
	}

	private void OnApplicationQuit()
	{
		profile.colorGrading.settings = origSetting;
	}

	public void StartPlagueEffect()
	{
		StartCoroutine(Plague(0.5f));
	}

	public void StopPlagueEffect()
	{
		StartCoroutine(Plague(-0.5f));
	}

	public void StartWarEffect()
	{
		StartCoroutine(War(0.5f, -0.33f));
	}

	public void StopWarEffect()
	{
		StartCoroutine(War(-0.5f, 0.33f));
	}

	IEnumerator Plague(float gVal)
	{
		var settings = profile.colorGrading.settings;

		var time = 1f;

		while (time > 0)
		{
			time -= Time.deltaTime;
			settings = profile.colorGrading.settings;
			settings.colorWheels.log.slope.g += Time.deltaTime * gVal;
			profile.colorGrading.settings = settings;
			yield return null;
		}
	}

	IEnumerator War(float rVal, float expVal)
	{
		var settings = profile.colorGrading.settings;

		var time = 1f;

		while (time > 0)
		{
			time -= Time.deltaTime;
			settings = profile.colorGrading.settings;
			settings.colorWheels.log.slope.r += Time.deltaTime * rVal;
			settings.basic.postExposure += Time.deltaTime * expVal;
			profile.colorGrading.settings = settings;
			yield return null;
		}
	}
}
