using UnityEngine;
using UnityUtility;

public class StarvingMinigame : BaseMinigame
{
	public override bool IsPlaying { get; protected set; }

	public override event Action<bool> OnGameFinished;

	public override void StartPlay(int choice)
	{
		throw new System.NotImplementedException();
	}

	public override void StopPlay()
	{
		throw new System.NotImplementedException();
	}
}
