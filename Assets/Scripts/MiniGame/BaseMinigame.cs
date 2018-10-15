using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public abstract class BaseMinigame : MonoBehaviour
{
	public abstract bool IsPlaying { get; protected set; }
	public abstract void StartPlay(int choice);
	public abstract void StopPlay();
	public abstract event Action<bool> OnGameFinished;
}
