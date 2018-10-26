using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FamilyManager : MonoBehaviour
{
	[SerializeField] List<FamilyMember> familymembers = new List<FamilyMember>();

	public List<FamilyMember> FamilyMembers
	{
		get { return familymembers; }
	}
}
