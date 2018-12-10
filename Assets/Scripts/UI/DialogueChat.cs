using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChat : MonoBehaviour
{
	[SerializeField] int index;
	[SerializeField] List<string> dialogues;

	public void StartChat()
	{
		DialogueWindow.Instance.Open(FamilyManager.FamilyMembers[index].portrait, dialogues[Random.Range(0, dialogues.Count)]);
	}
}
