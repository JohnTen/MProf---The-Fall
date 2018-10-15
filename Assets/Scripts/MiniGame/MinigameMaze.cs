using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameMaze : MonoBehaviour
{
	[SerializeField] float pcMovingSpeed;
	[SerializeField] float maxMovement;
	[SerializeField] Transform startingPoint;

	public float PCMovingSpeed
	{
		get { return pcMovingSpeed; }
	}

	public float MaxMovement
	{
		get { return maxMovement; }
	}

	public Transform StartingPoint
	{
		get { return startingPoint; }
	}
}
