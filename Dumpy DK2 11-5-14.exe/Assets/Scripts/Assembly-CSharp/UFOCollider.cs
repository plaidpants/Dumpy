using UnityEngine;

public class UFOCollider : MonoBehaviour
{
	private void Start()
	{
		SphereCollider sphereCollider = GetComponent<SphereCollider>();
		if (sphereCollider != null)
		{
			Destroy(sphereCollider);
		}
	}
}
