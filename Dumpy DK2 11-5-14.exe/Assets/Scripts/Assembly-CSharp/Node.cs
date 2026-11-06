using UnityEngine;

public class Node : MonoBehaviour
{
	private Vector3 startPos;

	private void Start()
	{
		startPos = transform.position;
	}

	private void Update()
	{
		transform.position = startPos;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, 1f);
	}
}
