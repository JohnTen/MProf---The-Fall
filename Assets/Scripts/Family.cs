using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum FamilyType
{
	Wife,
	Daughter,
	Son,
}

[System.Serializable]
public class FamilyMember
{
	public string name;
	public FamilyType type;
	public int requiredWheat;
	public int hunger;
	public float mentalHealth;
}
