using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FieldManager : MonoSingleton<FieldManager>
{
	[SerializeField] PloughingMinigame ploughingMinigame;
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

	public void StartPlantMinigame(int level, Action<float> onGameFinished)
	{
		ploughingMinigame.StartPlay(level);
		if (ploughingMinigame.IsPlaying)
			ploughingMinigame.OnGameFinished += onGameFinished;
		else
			onGameFinished.Invoke(1);
	}

	public void StartHarvestMinigame(Action<float> onGameFinished)
	{
		harvestingMinigame.StartPlay(0);
		if (harvestingMinigame.IsPlaying)
			harvestingMinigame.OnGameFinished += onGameFinished;
		else
			onGameFinished.Invoke(1);
	}

	protected override void Awake()
	{
		base.Awake();
		GetComponentsInChildren(fieldBlocks);
	}
}
