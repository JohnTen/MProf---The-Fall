using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FieldManager : MonoSingleton<FieldManager>
{
	[SerializeField] FarmingMinigame minigame;
	[SerializeField] List<FieldBlock> fieldBlocks = new List<FieldBlock>();

	public List<FieldBlock> FieldBlocks
	{
		get { return Instance.fieldBlocks; }
	}

	public void Harvest()
	{
		foreach (var fb in FieldBlocks)
		{
			fb.Harvest();
		}

		GameDataManager.UpdateValues();
	}

	public void StartPlantMinigame(int level, Action<bool> onGameFinished)
	{
		minigame.StartPlay(level);
		if (minigame.IsPlaying)
			minigame.OnGameFinished += onGameFinished;
		else
			onGameFinished.Invoke(true);
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponentsInChildren(fieldBlocks);
	}
}
