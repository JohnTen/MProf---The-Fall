using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FieldManager : MonoSingleton<FieldManager>
{
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

	protected override void Awake()
	{
		base.Awake();
		GetComponentsInChildren(fieldBlocks);
	}
}
