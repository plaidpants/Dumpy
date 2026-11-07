using UnityEngine;
using UnityEngine.AI;

public class HighwayScript : MonoBehaviour
{
	public Transform[] highwayNodes;
	public GameObject[] vehiclePrefabs;
	public GameObject spawnObject;
	public float NextGeneration;
	private float timer;

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			timer = 0.5f;
			CreateVehicle();
		}
	}

	private void CreateVehicle()
	{
		if (vehiclePrefabs == null || vehiclePrefabs.Length == 0 || spawnObject == null || highwayNodes == null || highwayNodes.Length == 0)
		{
			return;
		}
		int num = Random.Range(0, vehiclePrefabs.Length);
		Vector3 position = spawnObject.transform.position;
		Quaternion rotation = spawnObject.transform.rotation;
		GameObject gameObject = Instantiate(vehiclePrefabs[num], position, rotation) as GameObject;
		gameObject.layer = LayerMask.NameToLayer("HighwayCar");
		gameObject.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);

		bool hasNavMesh = NavMesh.SamplePosition(position, out _, 2f, NavMesh.AllAreas);
		if (hasNavMesh)
		{
			NavMeshAgent agent = gameObject.AddComponent<NavMeshAgent>();
			if (agent != null && agent.isOnNavMesh)
			{
				agent.destination = highwayNodes[0].position;
				agent.speed = 150f;
				agent.angularSpeed = 500f;
				agent.acceleration = 300f;
				agent.height = 2f;
			}
		}

		gameObject.AddComponent<PushCar>();
		HighwayCar car = gameObject.AddComponent<HighwayCar>();
		car.highWayNodes = highwayNodes;
		car.currentNode = highwayNodes[0].gameObject;
	}
}
