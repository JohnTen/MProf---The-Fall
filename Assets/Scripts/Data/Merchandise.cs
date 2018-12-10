using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class Merchandise
{
	public string name;
	public Sprite pic;
	public int requiredWheat;
	public int requiredOat;
	public int requiredMilk;
	public int sanityBoost;
	public FamilyType familyMember;
}
