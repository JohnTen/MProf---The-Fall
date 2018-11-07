using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class Smoke : MonoBehaviour
{
	[SerializeField] Vector2 rate;
	ParticleSystem emitter;
	int emitterCount;
	float timer;
	
	void Awake()
	{
		emitter = GetComponent<ParticleSystem>();
		emitterCount = emitter.subEmitters.subEmittersCount;
	}
	
	void Update ()
	{
		if (Time.time < timer) return;

		timer = Time.time + rate.RandomBetween();
		emitter.TriggerSubEmitter(Random.Range(0, emitterCount));
	}
}
