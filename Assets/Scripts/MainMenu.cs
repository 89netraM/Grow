using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void Play()
	{
		SceneManager.LoadScene(LevelManager.NextLevelName());
	}

	public void Levels()
	{
		SceneManager.LoadScene(LevelManager.LevelsLevelName);
	}

	public void Exit()
	{
		Application.Quit();
	}
}