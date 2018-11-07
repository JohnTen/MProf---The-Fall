using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameMaze : MonoBehaviour
{
	[SerializeField] float pcMovingSpeed;
	[SerializeField] float timeLimit = 60;
	[SerializeField] Transform startingPoint;

	public float PCMovingSpeed
	{
		get { return pcMovingSpeed; }
	}

	public float TimeLimit
	{
		get { return timeLimit; }
	}

	public Transform StartingPoint
	{
		get { return startingPoint; }
	}
}
