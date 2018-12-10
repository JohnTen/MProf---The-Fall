using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum FamilyType
{
	None,
	PC,
	Wife,
	Daughter,
	Son,
}

[System.Serializable]
public class FamilyMember
{
	public string name;
	public FamilyType type;
	public int requiredWheat = 1;
	public float dyingRate = 0.3f;
	public int hunger = 0;
	public int mentalHealth = 100;
	public float mentalDyingRatio = 0.4f;
	public bool gone;
	public Sprite portrait;
}
