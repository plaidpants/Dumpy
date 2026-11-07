using UnityEngine;
using UnityEngine.AI;

public class HighwayCar : MonoBehaviour
{
	public Transform[] highWayNodes;
	public GameObject currentNode;
	private NavMeshAgent navMeshAgent;
	private float speed = 150f;
	private float maxLifeTime = 45f;
	private float lifeTimer = 0f;

	private void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= maxLifeTime)
		{
			Destroy(gameObject);
			return;
		}

		if (currentNode == null)
		{
			Destroy(gameObject);
			return;
		}

		if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
		{
			if (navMeshAgent.remainingDistance < 50f)
			{
				HighwayNode node = currentNode.GetComponent<HighwayNode>();
				if (node != null && node.next != null)
				{
					navMeshAgent.destination = node.next.transform.position;
					currentNode = node.next.gameObject;
				}
				else
				{
					Destroy(gameObject);
					return;
				}
			}
		}
		else
		{
			// Kinematic waypoint navigation along highway nodes
			Vector3 targetPos = currentNode.transform.position;
			Vector3 toTarget = targetPos - transform.position;

			if (toTarget.sqrMagnitude < 25f * 25f)
			{
				HighwayNode node = currentNode.GetComponent<HighwayNode>();
				if (node != null && node.next != null)
				{
					currentNode = node.next.gameObject;
					targetPos = currentNode.transform.position;
					toTarget = targetPos - transform.position;
				}
				else
				{
					Destroy(gameObject);
					return;
				}
			}

			transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

			if (toTarget.sqrMagnitude > 0.01f)
			{
				Quaternion targetRot = Quaternion.LookRotation(toTarget);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360f * Time.deltaTime);
			}
		}
	}
}
