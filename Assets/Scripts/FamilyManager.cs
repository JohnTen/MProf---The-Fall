using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FamilyManager : MonoSingleton<FamilyManager>
{
	[SerializeField] GameObject[] familyMemberObjects;

	[SerializeField] List<FamilyMember> familyMembers = new List<FamilyMember>();

	public static List<FamilyMember> FamilyMembers
	{
		get { return Instance.familyMembers; }
	}

	private void Update()
	{
		for (int i = 0; i < familyMembers.Count; i++)
		{
			familyMemberObjects[i].SetActive(!familyMembers[i].gone);
		}
	}
}
