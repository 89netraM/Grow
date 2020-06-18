using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class BoxMovement : MonoBehaviour
{
	private const int PlatformsLayer = 1 << 8;
	private const int PlayersLayer = 1 << 9;
	private const int SolidLayers = PlatformsLayer | PlayersLayer;

	public bool AllowedToMove { get; set; } = true;
	public bool OnTheMove { get; private set; } = false;
	public bool CanMove => AllowedToMove && !OnTheMove;

	[SerializeField]
	private GameObject arrowPrefab;

	private new Rigidbody2D rigidbody;
	private new Collider2D collider;

	[SerializeField]
	private float powerRangeMin;
	[SerializeField]
	private float powerRangeMax;

	private Coroutine activeMoveCoroutine;

	[SerializeField]
	private float forcePower;
	[SerializeField]
	private AnimationCurve forceCurve;
	[SerializeField]
	private float minimumMovementVelocity;

	[SerializeField]
	private AnimationCurve scaleCurve;
	[SerializeField]
	private float maxScale;

	private TrailRenderer trail;
	[SerializeField]
	private float trailLength;

	private AudioSource audioSource;
	[SerializeField]
	private AudioClip jumpClip;
	[SerializeField]
	private AudioClip landClip;
	[SerializeField]
	private AudioClip resetClip;

	public event Action Reset;
	private Vector3 startingPosition;
	private float startingScale;
	[SerializeField]
	private float resetDelay;

	private GameController gameController;

	private float scale
	{
		get => transform.localScale.y;
		set
		{
			transform.localScale = Vector3.one * value;

			SetTrailWidth(value);
		}
	}

	public void Awake()
	{
		rigidbody = GetComponent<Rigidbody2D>();
		audioSource = GetComponent<AudioSource>();
		collider = GetComponents<Collider2D>().First(c => !c.isTrigger);
		trail = GetComponentInChildren<TrailRenderer>();
		gameController = GameObject.FindObjectOfType<GameController>();

		SetTrailWidth(scale);

		startingPosition = transform.position;
		startingScale = scale;
	}

	public void Start()
	{
		StartCoroutine(DetectNonVisibility());
	}

	public void OnMouseDown()
	{
		if (CanMove && IsStationary())
		{
			gameController.ActiveBox = this;
			activeMoveCoroutine = StartCoroutine(Move());
		}
	}

	private IEnumerator Move()
	{
		OnTheMove = true;

		GameObject arrowObject = Instantiate(arrowPrefab, transform.position, Quaternion.identity, transform);
		Arrow arrow = arrowObject.GetComponent<Arrow>();

		Vector2 vector;
		do
		{
			vector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			if (vector.magnitude > powerRangeMax)
			{
				vector = vector.normalized * powerRangeMax;
			}

			arrow.SetDirectionAndlength(vector);
			arrow.IsValid = vector.magnitude >= powerRangeMin;

			yield return null;
		}
		while (Input.GetMouseButton(0));

		if (vector.magnitude >= powerRangeMin)
		{
			rigidbody.velocity = vector * forcePower * forceCurve.Evaluate(Mathf.InverseLerp(1.0f, maxScale, scale));

			{
				float volume = Mathf.Lerp(0.5f, 1.0f, Mathf.InverseLerp(powerRangeMin, powerRangeMax, vector.magnitude)) * forceCurve.Evaluate(Mathf.InverseLerp(1.0f, maxScale, scale));
				audioSource.PlayOneShot(jumpClip, volume);
			}

			arrow.Remove();

			yield return new WaitUntil(IsStationary);

			scale = Mathf.Min(maxScale, scale * scaleCurve.Evaluate(Mathf.InverseLerp(powerRangeMin, powerRangeMax, vector.magnitude)));
		}
		else
		{
			arrow.Destroy();
		}

		EndMove();
	}
	public bool IsStationary()
	{
		return rigidbody.velocity.magnitude <= minimumMovementVelocity &&
			rigidbody.angularVelocity <= minimumMovementVelocity &&
			collider.IsTouchingLayers(SolidLayers);
	}
	private void EndMove()
	{
		OnTheMove = false;
		activeMoveCoroutine = null;
	}
	private void StopMove()
	{
		if (activeMoveCoroutine is Coroutine)
		{
			StopCoroutine(activeMoveCoroutine);
			EndMove();
		}
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		audioSource.PlayOneShot(landClip, Mathf.InverseLerp(0.0f, 5.0f, collision.relativeVelocity.magnitude));
	}

	public bool IsVisible()
	{
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
		return 0.0f < viewportPosition.x && viewportPosition.x < 1.0f &&
			0.0f < viewportPosition.y && viewportPosition.y < 1.0f;
	}
	private IEnumerator DetectNonVisibility()
	{
		while (true)
		{
			if (IsVisible())
			{
				yield return null;
			}
			else
			{
				IEnumerator enumerator = Destroy();
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
		}
	}
	private IEnumerator Destroy()
	{
		yield return new WaitForSecondsRealtime(resetDelay);

		if (!IsVisible())
		{
			ResetState();
		}
	}
	public void ResetState()
	{
		if (Reset is Action)
		{
			Reset.Invoke();
		}

		audioSource.PlayOneShot(resetClip);

		StopMove();

		scale = startingScale;
		transform.eulerAngles = Vector3.zero;
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0.0f;
		ReSpawn();
	}
	private void ReSpawn()
	{
		Vector3 closestSpawnPoint = startingPosition;

		if (Physics2D.OverlapPoint(startingPosition) is Collider2D blockingCollider && blockingCollider.transform != transform)
		{
			IEnumerable<Vector3> possibleSpawnPoints = new[]
			{
				new Vector3(blockingCollider.bounds.center.x - (collider.bounds.extents.x + blockingCollider.bounds.extents.x), startingPosition.y), // Left
				new Vector3(blockingCollider.bounds.center.x + (collider.bounds.extents.x + blockingCollider.bounds.extents.x), startingPosition.y), // Right
				new Vector3(startingPosition.x, blockingCollider.bounds.center.y + (collider.bounds.extents.y + blockingCollider.bounds.extents.y)), // Above
			};
			closestSpawnPoint = possibleSpawnPoints.Aggregate((c, a) => (c - startingPosition).sqrMagnitude < (a - startingPosition).sqrMagnitude ? c : a);
		}

		transform.position = closestSpawnPoint;
	}

	private void SetTrailWidth(float width)
	{
		trail.widthMultiplier = width;
		trail.time = width * trailLength;
	}

	public void Shrink()
	{
		scale = 1.0f;
		rigidbody.velocity = Vector2.zero;
		StopMove();
	}

	public void OnValidate()
	{
		powerRangeMin = Mathf.Max(0.0f, powerRangeMin);
		powerRangeMax = Mathf.Max(powerRangeMin, powerRangeMax);

		forcePower = Mathf.Max(0.0f, forcePower);

		minimumMovementVelocity = Mathf.Max(0.0f, minimumMovementVelocity);

		maxScale = Mathf.Max(scale, maxScale);

		trailLength = Mathf.Max(0.0f, trailLength);
	}
}