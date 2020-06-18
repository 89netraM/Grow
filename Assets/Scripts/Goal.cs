using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class Goal : MonoBehaviour
{
	public const string PlayerTag = "Player";

	public bool IsFinished { get; private set; } = false;
	public event Action Finished;

	private BoxMovement activatingBox;

	private ParticleSystem.MainModule psMain;
	private Color startingColor;
	[SerializeField]
	private Color finishedColor;

	private AudioSource audioSource;

	private Coroutine activeTriggerCoroutine;

	public void Awake()
	{
		psMain = GetComponent<ParticleSystem>().main;
		startingColor = psMain.startColor.color;

		audioSource = GetComponent<AudioSource>();
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (!IsFinished && !(activeTriggerCoroutine is Coroutine) && collision.GetComponent<BoxMovement>() is BoxMovement box)
		{
			activeTriggerCoroutine = StartCoroutine(Trigger(box));
		}
	}

	private IEnumerator Trigger(BoxMovement box)
	{
		activatingBox = box;
		activatingBox.Reset += Box_Reset;

		psMain.startColor = finishedColor;
		audioSource.Play();

		yield return new WaitUntil(() => box.IsStationary() && box.IsVisible());

		IsFinished = true;

		if (Finished is Action)
		{
			Finished.Invoke();
		}

		activeTriggerCoroutine = null;
	}

	private void Box_Reset()
	{
		if (activeTriggerCoroutine is Coroutine)
		{
			StopCoroutine(activeTriggerCoroutine);
			activeTriggerCoroutine = null;
		}

		activatingBox.Reset -= Box_Reset;

		IsFinished = false;
		psMain.startColor = startingColor;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}