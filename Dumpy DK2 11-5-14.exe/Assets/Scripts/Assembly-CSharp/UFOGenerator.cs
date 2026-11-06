using UnityEngine;

public class UFOGenerator : MonoBehaviour
{
	public float NextGeneration;
	public GameObject UFOPrefab;
	public Transform[] nodes01;
	public Transform[] nodes02;
	public Transform[] nodes03;
	public Transform[] nodes04;
	public int UFOMoveForce;
	public int UFONodeDistance;
	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= NextGeneration)
		{
			NextGeneration = Time.deltaTime + NextGeneration;
			timer -= NextGeneration;
			createUFO();
		}
	}

	private void createUFO()
	{
		if (UFOPrefab == null) return;
		GameObject gameObject = Instantiate(UFOPrefab, transform.position, transform.rotation) as GameObject;
		Transform ufoTransform = gameObject.transform.Find("UFO");
		if (ufoTransform != null)
		{
			UFO component = ufoTransform.GetComponent<UFO>();
			if (component != null)
			{
				int num = Random.Range(0, 4);
				if (num == 0) component.nodes = nodes01;
				if (num == 1) component.nodes = nodes04;
				if (num == 2) component.nodes = nodes03;
				if (num == 3) component.nodes = nodes02;
				component.moving = true;
				component.moveForce = UFOMoveForce;
				component.nodeDist = UFONodeDistance;
			}
			Transform ufoGeo = ufoTransform.Find("ufo/UFOgeo");
			if (ufoGeo != null)
			{
				MeshRenderer mr = ufoGeo.GetComponent<MeshRenderer>();
				if (mr != null) mr.enabled = true;
			}
			Transform tractor = ufoTransform.Find("Tractorbeam");
			if (tractor != null)
			{
				Destroy(tractor.gameObject);
			}
		}
	}
}
