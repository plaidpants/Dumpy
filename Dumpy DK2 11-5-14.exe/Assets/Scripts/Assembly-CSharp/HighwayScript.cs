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
		NavMeshAgent agent = gameObject.AddComponent<NavMeshAgent>();
		gameObject.AddComponent<PushCar>();
		agent.destination = highwayNodes[0].position;
		agent.speed = 150f;
		agent.angularSpeed = 500f;
		agent.acceleration = 300f;
		agent.height = 2f;
		HighwayCar car = gameObject.AddComponent<HighwayCar>();
		car.highWayNodes = highwayNodes;
		car.currentNode = highwayNodes[0].gameObject;
	}
}
