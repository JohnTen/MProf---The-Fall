using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityUtility;

[System.Serializable]
public class SoundClips
{
	public string label;
	public AudioMixerGroup output;
	public AudioSource source;
	public bool mute;
	public bool playOnAwake;
	public bool loop;

	[Range(0f, 1f)] public float volume = 1;
	[Range(-3f, 3f)] public float pitch = 1;
	public AudioClip[] sounds;

	public bool IsActivated { get; private set; }

	public bool IsPlaying
	{
		get { return source.isPlaying; }
	}

	public void SetSource(AudioSource s)
	{
		source = s;
		source.outputAudioMixerGroup = output;
		source.mute = mute;
		source.playOnAwake = playOnAwake;
		source.loop = loop;
		source.volume = volume;
		source.pitch = pitch;
	}

	public void Play()
	{
		if (sounds.Length <= 0) return;
		var index = Random.Range(0, sounds.Length);

		SetSource(source);
		source.clip = sounds[index];
		source.Play();
		IsActivated = true;
	}

	public void Stop()
	{
		source.Stop();
		IsActivated = false;
	}
}

public class SoundManager : GlobalSingleton<SoundManager>
{
	[SerializeField] GameObject soundObject;
	[SerializeField] SoundClips[] clips;
	[SerializeField] List<AudioSource> sources;

	protected override void Awake()
	{
		base.Awake();

		soundObject = new GameObject("Sound Object");
		soundObject.transform.SetParent(GlobalObject.Instance.transform);
		sources = new List<AudioSource>();

		for (int i = 0; i < clips.Length; i ++)
		{
			if (clips[i].source != null) continue;

			clips[i].SetSource(soundObject.AddComponent<AudioSource>());
			if (clips[i].playOnAwake)
				clips[i].Play();

			sources.Add(clips[i].source);
		}
	}

	protected void Update()
	{
		foreach (var s in clips)
		{
			s.SetSource(s.source);
			if (s.IsPlaying) continue;

			if (s.IsActivated && s.loop)
				s.Play();
		}
	}

	public static void Play(string label)
	{
		Instance._Play(label);
	}

	public static void Stop(string label)
	{
		Instance._Stop(label);
	}

	public static bool IsPlaying(string label)
	{
		foreach (var s in Instance.clips)
		{
			if (s.label != label)
				continue;

			return s.IsPlaying;
		}

		return false;
	}

	public void _Play(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			s.Play();
		}
	}

	public void _Stop(string label)
	{
		foreach (var s in clips)
		{
			if (s.label != label)
				continue;

			s.Stop();
		}
	}
}
