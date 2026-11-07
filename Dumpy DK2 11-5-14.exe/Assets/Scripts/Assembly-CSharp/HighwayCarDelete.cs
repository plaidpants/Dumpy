using UnityEngine;
using UnityEngine.AI;

public class HighwayCarDelete : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider != null && collider.gameObject != null)
		{
			if (collider.GetComponent<HighwayCar>() != null || collider.GetComponent<NavMeshAgent>() != null)
			{
				Destroy(collider.gameObject);
			}
		}
	}
}
