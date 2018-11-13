using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class FarmingMinigame : BaseMinigame
{
	enum CurrentController
	{
		None,
		Up,
		Down,
		Right,
		Left,
		ArrowKeys
	}

	[SerializeField] RectTransform pc;
	[SerializeField] RectTransform timeMeter;
	[SerializeField] RectTransform growMeter;
	[SerializeField] RectTransform mazeBase;
	[SerializeField] List<MinigameMaze> level1Mazes;
	[SerializeField] List<MinigameMaze> level2Mazes;
	[SerializeField] List<MinigameMaze> level3Mazes;
	[SerializeField] List<MinigameMaze> level4Mazes;

	[Header("Debug")]
	[SerializeField] float currentMovement;
	[SerializeField] float timeMeterSize;
	[SerializeField] float growMeterSize;
	[SerializeField] Canvas canvas;
	[SerializeField] MinigameMaze maze;
	[SerializeField] List<RectTransform> pathes = new List<RectTransform>();
	[SerializeField] bool playing;
	[SerializeField] CurrentController movingDir;

	public override bool IsPlaying
	{
		get { return playing; }
		protected set { playing = value; }
	}

	public List<MinigameMaze> Level1Mazes
	{
		get { return level1Mazes; }
	}

	public List<MinigameMaze> Level2Mazes
	{
		get { return level2Mazes; }
	}

	public List<MinigameMaze> Level3Mazes
	{
		get { return level3Mazes; }
	}

	public List<MinigameMaze> Level4Mazes
	{
		get { return level4Mazes; }
	}

	public override event Action<bool> OnGameFinished;

	public void Display(bool display)
	{
		if (canvas == null)
			canvas = GetComponent<Canvas>();

		canvas.enabled = display;
	}

	public override void StartPlay(int choice)
	{
		maze = LoadMaze(choice);
		if (maze == null)
			return;

		var size = timeMeter.sizeDelta;
		size.y = timeMeterSize;
		timeMeter.sizeDelta = size;
		
		CollectPathes();
		IsPlaying = true;
		canvas.enabled = true;
		OnGameFinished = null;
		maze.StartingPoint.gameObject.SetActive(false);
		pc.position = maze.StartingPoint.position;
		TimeManager.FreezeTime();

		currentMovement = 0;
	}

	public override void StopPlay()
	{
		IsPlaying = false;
		canvas.enabled = false;
		TimeManager.UnfreezeTime();
		MessageBox.OnContinue -= StopPlay;
	}

	public void MoveUp()
	{
		movingDir = CurrentController.Up;
	}

	public void MoveRight()
	{
		movingDir = CurrentController.Right;
	}

	public void MoveDown()
	{
		movingDir = CurrentController.Down;
	}

	public void MoveLeft()
	{
		movingDir = CurrentController.Left;
	}

	public void EndMove()
	{
		movingDir = CurrentController.None;
	}

	public MinigameMaze LoadMaze(int level)
	{
		int randomIndex = -1;
		switch (level)
		{
			case 0:
				if (level1Mazes.Count <= 0) break;
				randomIndex = Random.Range(0, level1Mazes.Count);
				break;

			case 1:
				if (level2Mazes.Count <= 0) break;
				randomIndex = Random.Range(0, level2Mazes.Count);
				break;

			case 2:
				if (level3Mazes.Count <= 0) break;
				randomIndex = Random.Range(0, level3Mazes.Count);
				break;

			case 3:
				if (level4Mazes.Count <= 0) break;
				randomIndex = Random.Range(0, level4Mazes.Count);
				break;
		}

		return LoadMaze(level, randomIndex);
	}

	public MinigameMaze LoadMaze(int level, int index)
	{
		if (maze != null)
			DestroyImmediate(maze.gameObject);

		if (pathes.Count > 0)
		{
			foreach (var p in pathes)
			{
				if (p == null) continue;
				p.gameObject.SetActive(false);
			}
		}

		maze = null;

		if (level < 0 || index < 0) return maze;

		switch (level)
		{
			case 0:
				if (level1Mazes.Count <= 0) break;
				maze = level1Mazes[index];
				break;

			case 1:
				if (level2Mazes.Count <= 0) break;
				maze = level2Mazes[index];
				break;

			case 2:
				if (level3Mazes.Count <= 0) break;
				maze = level3Mazes[index];
				break;

			case 3:
				if (level4Mazes.Count <= 0) break;
				maze = level4Mazes[index];
				break;
		}
		if (maze == null) return maze;

		maze = Instantiate(maze.gameObject).GetComponent<MinigameMaze>();
		maze.transform.SetParent(mazeBase, false);
		maze.transform.SetAsFirstSibling();
		maze.StartingPoint.gameObject.SetActive(true);

		return maze;
	}

	void CollectPathes()
	{
		pathes.Clear();
		mazeBase.GetComponentsInChildren(pathes);
		for (int i = 0; i < pathes.Count; i++)
		{
			if (!pathes[i].gameObject.activeInHierarchy || 
				(pathes[i].tag != "MazePath" && 
				pathes[i].tag != "MazeEnd"))
			{
				pathes.RemoveAt(i);
				i--;
			}
		}
	}

	Vector2 RotateAround(Vector2 center, Vector2 point, float degree)
	{
		degree *= -Mathf.Deg2Rad;
		var centerToPoint = point - center;
		var x = centerToPoint.x * Mathf.Cos(degree) - centerToPoint.y * Mathf.Sin(degree);
		var y = centerToPoint.x * Mathf.Sin(degree) + centerToPoint.y * Mathf.Cos(degree);

		return center + new Vector2(x, y);
	}

	Vector2 TransformToRectSpace(RectTransform rect, Vector2 point)
	{
		return point;
	}

	bool IsContained(RectTransform rect1, Vector2 point)
	{
		point = RotateAround(rect1.rect.center, point, rect1.eulerAngles.z);

		return rect1.rect.Contains(point);
	}

	private void Move()
	{
		var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (movement != Vector2.zero)
			movingDir = CurrentController.ArrowKeys;
		else if (movingDir == CurrentController.ArrowKeys)
			movingDir = CurrentController.None;

		switch (movingDir)
		{
			case CurrentController.None:
				return;

			case CurrentController.Up:
				movement = Vector2.up;
				break;
			case CurrentController.Down:
				movement = Vector2.down;
				break;
			case CurrentController.Left:
				movement = Vector2.left;
				break;
			case CurrentController.Right:
				movement = Vector2.right;
				break;
			case CurrentController.ArrowKeys:
				movement.Normalize();
				break;
		}

		currentMovement += maze.PCMovingSpeed * Time.unscaledDeltaTime * 0.1f;
		pc.transform.Translate(movement * maze.PCMovingSpeed * Time.unscaledDeltaTime);
	}

	private bool IsPCWithinBoundary()
	{
		bool upperLeftContained = false;
		bool upperRightContained = false;
		bool bottomLeftContained = false;
		bool bottomRightContained = false;
		bool upperMiddleContained = false;
		bool middleRightContained = false;
		bool middleLeftContained = false;
		bool bottomMiddleContained = false;

		Vector2 upperLeftCorner = new Vector2(pc.rect.yMax, pc.rect.xMin);
		Vector2 upperRightCorner = new Vector2(pc.rect.yMax, pc.rect.xMax);
		Vector2 bottomLeftCorner = new Vector2(pc.rect.yMin, pc.rect.xMin);
		Vector2 bottomRightCorner = new Vector2(pc.rect.yMin, pc.rect.xMax);
		Vector2 upperMiddleCorner = new Vector2(pc.rect.yMax, (pc.rect.xMax + pc.rect.xMin)*0.5f);
		Vector2 middleRightCorner = new Vector2((pc.rect.yMax + pc.rect.yMin) * 0.5f, pc.rect.xMax);
		Vector2 middleLeftCorner = new Vector2((pc.rect.yMax + pc.rect.yMin) * 0.5f, pc.rect.xMin);
		Vector2 bottomMiddleCorner = new Vector2(pc.rect.yMin, (pc.rect.xMax + pc.rect.xMin) * 0.5f);

		foreach (var p in pathes)
		{
			Vector2 offset = pc.anchoredPosition - p.anchoredPosition;
			var point = Vector2.zero;

			if (!upperLeftContained)
			{
				point = upperLeftCorner + offset;
				upperLeftContained = IsContained(p, point);
			}

			if (!upperRightContained)
			{
				point = upperRightCorner + offset;
				upperRightContained = IsContained(p, point);
			}

			if (!bottomLeftContained)
			{
				point = bottomLeftCorner + offset;
				bottomLeftContained = IsContained(p, point);
			}

			if (!bottomRightContained)
			{
				point = bottomRightCorner + offset;
				bottomRightContained = IsContained(p, point);
			}

			if (!upperMiddleContained)
			{
				point = upperMiddleCorner + offset;
				upperMiddleContained = IsContained(p, point);
			}

			if (!bottomMiddleContained)
			{
				point = bottomMiddleCorner + offset;
				bottomMiddleContained = IsContained(p, point);
			}

			if (!middleLeftContained)
			{
				point = middleLeftCorner + offset;
				middleLeftContained = IsContained(p, point);
			}

			if (!middleRightContained)
			{
				point = middleRightCorner + offset;
				middleRightContained = IsContained(p, point);
			}

			if (upperLeftContained &&
				upperRightContained &&
				bottomLeftContained &&
				bottomRightContained &&
				middleLeftContained &&
				middleRightContained &&
				upperMiddleContained &&
				bottomMiddleContained)
				return true;
		}

		return false;
	}

	private bool IsPCWithinEndPoint()
	{
		bool maxContained = false;
		bool minContained = false;
		
		foreach (var p in pathes)
		{
			if (p.tag != "MazeEnd") continue;
			Vector2 offset = pc.anchoredPosition - p.anchoredPosition;
			var point = Vector2.zero;

			if (!maxContained)
			{
				point = pc.rect.max + offset;
				maxContained = IsContained(p, point);
			}

			if (!minContained)
			{
				point = pc.rect.min + offset;
				minContained = IsContained(p, point);
			}

			if (maxContained &&
				minContained)
				return true;
		}

		return false;
	}

	private void Awake()
	{
		timeMeterSize = timeMeter.rect.height;
		growMeterSize = growMeter.rect.height;

		if (canvas == null)
			canvas = GetComponent<Canvas>();
		canvas.enabled = false;
	}

	private void Start()
	{
		timeMeterSize = timeMeter.rect.height;
		growMeterSize = growMeter.rect.height;
		CollectPathes();
	}

	private void Update()
	{
		if (!IsPlaying) return;

		var origPos = pc.position;
		Move();
		var size = timeMeter.sizeDelta;
		size.y -= (Time.unscaledDeltaTime / maze.TimeLimit) * timeMeterSize;
		timeMeter.sizeDelta = size;

		if (size.y <= 0 || Input.GetKeyDown(KeyCode.P))
		{
			MessageBox.DisplayMessage("Minigame failed!", "Your new planted crops is doomed and nothing can be plant here in this week!");
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(false);
			}
			return;
		}

		if (IsPCWithinEndPoint() || Input.GetKeyDown(KeyCode.O))
		{
			MessageBox.DisplayMessage("Minigame Won!", "Your crops is safely planted!");
			IsPlaying = false;
			MessageBox.OnContinue += StopPlay;
			if (OnGameFinished != null)
			{
				OnGameFinished.Invoke(true);
			}
			return;
		}

		if (!IsPCWithinBoundary())
		{
			pc.position = origPos;
		}
	}
}
