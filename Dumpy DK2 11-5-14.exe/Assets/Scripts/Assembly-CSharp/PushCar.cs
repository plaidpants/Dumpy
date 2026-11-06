using UnityEngine;
using UnityEngine.AI;

public class PushCar : MonoBehaviour
{
	private NavMeshAgent navMeshAgent;
	private Rigidbody rig;
	private bool hit;
	private float lifeLeft;

	private void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		rig = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (hit)
		{
			lifeLeft -= Time.deltaTime;
		}
		if (hit && lifeLeft <= 0f)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision otherObject)
	{
		if (navMeshAgent != null && otherObject.gameObject.layer != LayerMask.NameToLayer("Ground") && !hit)
		{
			navMeshAgent.isStopped = true;
			Destroy(navMeshAgent);
			Vector3 velocity = (otherObject.collider != null && otherObject.collider.attachedRigidbody != null) 
				? otherObject.collider.attachedRigidbody.velocity 
				: Vector3.zero;
			if (rig != null)
			{
				rig.AddForce(velocity * 2000f);
				rig.AddForce(new Vector3(Random.Range(1500, 2000), Random.Range(1500, 2000), Random.Range(1500, 2000)));
			}
			if (!hit)
			{
				StartTimer();
			}
		}
	}

	private void StartTimer()
	{
		hit = true;
		lifeLeft = 5f;
	}
}
