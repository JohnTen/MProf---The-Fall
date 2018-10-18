using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FieldManager : MonoSingleton<FieldManager>
{
	[SerializeField] FarmingMinigame farmingMinigame;
	[SerializeField] HarvestingMinigame harvestingMinigame;
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
		farmingMinigame.StartPlay(level);
		if (farmingMinigame.IsPlaying)
			farmingMinigame.OnGameFinished += onGameFinished;
		else
			onGameFinished.Invoke(true);
	}

	public void StartHarvestMinigame(Action<bool> onGameFinished)
	{
		harvestingMinigame.StartPlay(0);
		if (harvestingMinigame.IsPlaying)
			harvestingMinigame.OnGameFinished += onGameFinished;
		else
			onGameFinished.Invoke(true);
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponentsInChildren(fieldBlocks);
	}
}
