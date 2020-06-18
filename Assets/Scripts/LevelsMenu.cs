using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject levelButtonPrefab;
	[SerializeField]
	private RectTransform scrollViewContent;
	[SerializeField]
	private int rowCount;
	[SerializeField]
	private float gapWidth;

	public void Awake()
	{
		for (int i = 0; i < LevelManager.OrderedLevelNames.Length; i++)
		{
			string levelName = LevelManager.OrderedLevelNames[i];
			GameObject levelObject = Instantiate(levelButtonPrefab, scrollViewContent);
			RectTransform levelTransform = levelObject.GetComponent<RectTransform>();
			LevelButton levelButton = levelObject.GetComponent<LevelButton>();

			levelTransform.anchoredPosition = Vector2.right * (levelTransform.sizeDelta.x + gapWidth) * (i / rowCount) + Vector2.down * (levelTransform.sizeDelta.y + gapWidth) * (i % rowCount);
			levelButton.SetLevel(levelName);

			scrollViewContent.sizeDelta = Vector2.right * (levelTransform.anchoredPosition.x + levelTransform.sizeDelta.x);
		}
	}

	public void Back()
	{
		SceneManager.LoadScene(LevelManager.MenuLevelName);
	}
}