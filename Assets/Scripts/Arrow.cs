using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Arrow : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Color validColor;
	[SerializeField]
	private Color invalidColor;

	public bool IsValid
	{
		get => spriteRenderer.color == validColor;
		set => spriteRenderer.color = value ? validColor : invalidColor;
	}

	public void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = invalidColor;
		transform.localScale = new Vector3(1.0f / transform.parent.localScale.x, 1.0f / transform.parent.localScale.y, 1.0f / transform.parent.localScale.z);
	}

	/// <summary>
	/// Points the arrow in the direction of,
	/// and with length of <paramref name="vector"/>.
	/// </summary>
	public void SetDirectionAndlength(Vector2 vector)
	{
		spriteRenderer.size = new Vector2(spriteRenderer.size.x, vector.magnitude);
		transform.eulerAngles = new Vector3(0.0f, 0.0f, Vector2.SignedAngle(Vector2.up, vector));
	}

	public void Remove()
	{
		Destroy(gameObject);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}