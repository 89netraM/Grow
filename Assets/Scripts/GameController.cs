using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	private Goal[] goals;

	[SerializeField]
	private float victoryDelay;
	[SerializeField]
	private GameObject victoryPrefab;

	private string currentLevelName;

	public bool IsFinished { get; private set; } = false;
	public event Action Finished;

	public BoxMovement ActiveBox;

	public void Awake()
	{
		goals = GameObject.FindObjectsOfType<Goal>();
		foreach (Goal goal in goals)
		{
			goal.Finished += Goal_Finished;
		}

		currentLevelName = SceneManager.GetActiveScene().name;
	}

	public void Update()
	{
		if (!IsFinished)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Menu();
			}
			else if (Input.GetKeyDown(KeyCode.R) && ActiveBox is BoxMovement)
			{
				ActiveBox.ResetState();
			}
		}
	}

	public IEnumerator FinishLevel()
	{
		IsFinished = true;

		if (Finished is Action)
		{
			Finished.Invoke();
		}

		yield return new WaitForSecondsRealtime(victoryDelay);

		LevelManager.SetClearedLevel(currentLevelName);
		Instantiate(victoryPrefab);
	}

	private void Goal_Finished()
	{
		if (goals.All(g => g.IsFinished))
		{
			StartCoroutine(FinishLevel());
		}
	}

	public void Menu()
	{
		SceneManager.LoadScene(LevelManager.MenuLevelName);
	}
}