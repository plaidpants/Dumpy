using UnityEngine;

public class UVmover : MonoBehaviour
{
	public bool moving;
	public float speed;
	private Vector2 currentOffset;
	private Renderer rend;

	private void Start()
	{
		rend = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (moving && rend != null && rend.material != null)
		{
			currentOffset = new Vector2(currentOffset.x, currentOffset.y - speed * Time.deltaTime);
			rend.material.SetTextureOffset("_MainTex", currentOffset);
		}
	}
}
