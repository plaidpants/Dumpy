using UnityEngine;

public class ufoMother : Actor
{
	public GameObject endContainer;
	public Transform[] nodes;
	private float aniSpeed = 1f;
	private float rotSpeed = 0.07f;
	private bool moving;
	private bool movingStuff;
	private bool looping;
	private float moveForce = 30f;
	private float nodeDist = 5f;
	private float upForce = 540f;
	private float inForce = 40f;
	private int currentNode;
	private Transform[] allChildren;

	private new void Start()
	{
		base.Start();
		if (ani != null)
		{
			ani.speed = aniSpeed;
		}
		if (endContainer != null)
		{
			allChildren = endContainer.GetComponentsInChildren<Transform>();
		}
	}

	private void FixedUpdate()
	{
		transform.Rotate(Vector3.up * rotSpeed, Space.World);
		if (moving && nodes != null && nodes.Length > 0)
		{
			if (Vector3.Distance(transform.position, nodes[currentNode].position) < nodeDist)
			{
				if (currentNode != nodes.Length - 1)
				{
					currentNode++;
				}
				else if (looping)
				{
					currentNode = 0;
				}
				else
				{
					moving = false;
				}
			}
			if (rig != null && currentNode < nodes.Length && nodes[currentNode] != null)
			{
				rig.AddForce(moveForce * (nodes[currentNode].position - transform.position).normalized);
			}
		}
		if (!movingStuff || allChildren == null)
		{
			return;
		}
		foreach (Transform t in allChildren)
		{
			if (t != null)
			{
				Rigidbody rb = t.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.AddForce((transform.position - t.position).normalized * inForce);
					rb.AddForce(Vector3.up * upForce);
				}
			}
		}
	}

	public override void Trigger()
	{
		moving = true;
		movingStuff = true;
		Transform tractor = transform.Find("Tractorbeam");
		if (tractor != null)
		{
			tractor.gameObject.SetActive(true);
		}
		if (allChildren != null)
		{
			foreach (Transform t in allChildren)
			{
				if (t != null)
				{
					Rigidbody rb = t.GetComponent<Rigidbody>();
					if (rb != null)
					{
						rb.AddForce(Vector3.up * 10000f);
					}
				}
			}
		}
	}
}
