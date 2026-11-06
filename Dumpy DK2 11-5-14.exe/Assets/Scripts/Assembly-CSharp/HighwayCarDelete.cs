using UnityEngine;
using UnityEngine.AI;

public class HighwayCarDelete : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider != null && collider.gameObject != null)
		{
			NavMeshAgent component = collider.gameObject.GetComponent<NavMeshAgent>();
			if (component != null)
			{
				Destroy(collider.gameObject);
			}
		}
	}
}
