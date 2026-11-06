using UnityEngine;

public class Drop_Object : MonoBehaviour
{
	public float NextGeneration;
	public GameObject[] prefabArray;
	public GameObject spawnObject;
	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= NextGeneration)
		{
			timer = 0f;
			GenerateObject();
		}
	}

	private void GenerateObject()
	{
		if (prefabArray == null || prefabArray.Length == 0 || spawnObject == null)
		{
			return;
		}
		int num = Random.Range(0, prefabArray.Length);
		Vector3 position = spawnObject.transform.position;
		Quaternion rotation = spawnObject.transform.rotation;
		GameObject gameObject = Instantiate(prefabArray[num], position, rotation) as GameObject;
		MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
		if (mr != null)
		{
			mr.enabled = true;
		}
		Idle_Breakable ib = gameObject.GetComponent<Idle_Breakable>();
		if (ib != null)
		{
			Destroy(ib);
		}
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		if (rb != null)
		{
			if (!rb.useGravity)
			{
				rb.useGravity = true;
			}
			rb.mass = 1f;
			rb.AddForce(0f, -1000f, 0f);
		}
		Breakable b = gameObject.GetComponent<Breakable>();
		if (b != null)
		{
			b.strength = float.MaxValue;
		}
		BoxCollider bc = gameObject.GetComponent<BoxCollider>();
		if (bc != null)
		{
			Destroy(bc);
		}
		SphereCollider sc = gameObject.AddComponent<SphereCollider>();
		sc.radius *= 1.5f;
		DestroyOverTime dot = gameObject.AddComponent<DestroyOverTime>();
		dot.setTime(20f);
	}
}
