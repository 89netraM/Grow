using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Shrinker : MonoBehaviour
{
	private AudioSource audioSource;

	public void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<BoxMovement>() is BoxMovement box)
		{
			audioSource.Play();
			box.transform.position = transform.position;
			box.Shrink();
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(1.0f, 0.0f, 1.0f);
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}