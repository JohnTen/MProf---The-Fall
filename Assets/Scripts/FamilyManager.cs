using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FamilyManager : MonoSingleton<FamilyManager>
{
	[SerializeField] List<FamilyMember> familyMembers = new List<FamilyMember>();

	public static List<FamilyMember> FamilyMembers
	{
		get { return Instance.familyMembers; }
	}
}
