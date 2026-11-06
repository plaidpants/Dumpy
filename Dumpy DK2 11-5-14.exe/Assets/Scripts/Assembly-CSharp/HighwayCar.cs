using UnityEngine;
using UnityEngine.AI;

public class HighwayCar : MonoBehaviour
{
	public Transform[] highWayNodes;
	public GameObject currentNode;
	private NavMeshAgent navMeshAgent;

	private void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (navMeshAgent != null && navMeshAgent.remainingDistance < 50f && currentNode != null)
		{
			HighwayNode node = currentNode.GetComponent<HighwayNode>();
			if (node != null && node.next != null)
			{
				navMeshAgent.destination = node.next.transform.position;
				currentNode = node.next.gameObject;
			}
		}
	}
}
