using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FieldManager : MonoSingleton<FieldManager>
{
	[SerializeField] List<FieldBlock> fieldBlocks = new List<FieldBlock>();

	protected override void Awake()
	{
		base.Awake();
		GetComponentsInChildren(fieldBlocks);
	}

	public void Harvest()
	{
		foreach (var fb in fieldBlocks)
		{
			fb.Harvest();
		}

		GlobalValues.UpdateValues();
	}
}
