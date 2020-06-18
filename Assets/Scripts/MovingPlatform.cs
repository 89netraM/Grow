using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
	private const string playerTag = "Player";

	[SerializeField]
	private List<Vector2> points;
	[SerializeField]
	private float speed;

	private int pointIndex = 0;

	private List<Transform> riders = new List<Transform>();

	private new Collider2D collider;

	public void Awake()
	{
		collider = GetComponent<Collider2D>();
	}

	public void Start()
	{
		points.Insert(0, transform.position);
	}

	public void Update()
	{
		if (Vector2.Distance(transform.position, points[pointIndex]) < speed * Time.deltaTime * 2.0f)
		{
			pointIndex = (pointIndex + 1) % points.Count;
		}

		Vector3 delta = ((Vector3)points[pointIndex] - transform.position).normalized * speed * Time.deltaTime;

		transform.position += delta;
		foreach (Transform rider in riders)
		{
			rider.position += delta;
		}
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(playerTag) && IsAboveMe(other))
		{
			riders.Add(other.transform);
		}
	}
	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag(playerTag))
		{
			riders.Remove(other.transform);
		}
	}

	private bool IsAboveMe(Collider2D other)
	{
		ColliderDistance2D ditance = collider.Distance(other);

		return ditance.pointA.y > (transform.position.y + transform.localScale.y / 2.0f);
	}

	public void OnDrawGizmosSelected()
	{
		if (points.Count > 0)
		{
			Gizmos.color = new Color(1.0f, 0.5f, 0.0f);

			Gizmos.DrawLine(transform.position, points[0]);
			for (int i = 0; i < points.Count - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
			Gizmos.DrawLine(points[points.Count - 1], transform.position);
		}
	}
}