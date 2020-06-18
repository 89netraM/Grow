using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
	private Animation textAnimation;
	private new ParticleSystem particleSystem;

	[SerializeField]
	private float buttonsDelay;
	[SerializeField]
	private GameObject nextButton;
	[SerializeField]
	private GameObject[] buttons;

	private string currentLevelName;
	private string nextLevelName;

	public void Awake()
	{
		textAnimation = GetComponentInChildren<Animation>();
		particleSystem = GetComponentInChildren<ParticleSystem>();

		particleSystem.transform.position = Camera.main.ViewportToWorldPoint(Vector3.one / 2.0f);

		currentLevelName = SceneManager.GetActiveScene().name;
		nextLevelName = LevelManager.NextLevelName(currentLevelName);
	}

	public void Start()
	{
		StartCoroutine(DelayButtons());
	}

	private IEnumerator DelayButtons()
	{
		yield return new WaitWhile(() => textAnimation.isPlaying);
		yield return new WaitForSecondsRealtime(buttonsDelay);

		if (nextLevelName is string)
		{
			nextButton.SetActive(true);
		}
		foreach (GameObject button in buttons)
		{
			button.SetActive(true);
		}
	}

	public void Menu()
	{
		SceneManager.LoadScene(LevelManager.MenuLevelName);
	}
	public void Replay()
	{
		SceneManager.LoadScene(currentLevelName);
	}
	public void Next()
	{
		SceneManager.LoadScene(nextLevelName);
	}
}