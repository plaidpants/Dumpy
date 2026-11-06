using UnityEngine;

public class StartForce : MonoBehaviour
{
	public Vector3 direction;
	public float force;

	private void Start()
	{
		Vector3 normalized = direction.normalized;
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.AddForce(normalized.x * force, normalized.y * force, normalized.z * force);
		}
	}
}
