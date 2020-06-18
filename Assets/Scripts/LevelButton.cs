using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
	[SerializeField]
	private Text levelNameText;
	[SerializeField]
	private GameObject clearedObject;

	private string levelName;

	public void SetLevel(string levelName)
	{
		this.levelName = levelName;

		levelNameText.text = levelName;
		clearedObject.SetActive(LevelManager.HasClearedLevel(levelName));
	}

	public void LoadLevel()
	{
		SceneManager.LoadScene(levelName);
	}
}