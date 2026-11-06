using UnityEngine;

public class UFO : Actor
{
	private float rotSpeed = 1f;
	public bool moving;
	public bool looping;
	public float moveForce = 1000f;
	public float nodeDist = 5f;
	public Transform[] nodes;
	private int currentNode;

	private new void Start()
	{
		base.Start();
		Drop_Object drop = GetComponent<Drop_Object>();
		if (drop != null)
		{
			drop.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (!moving || nodes == null || nodes.Length == 0)
		{
			return;
		}
		transform.Rotate(Vector3.up * rotSpeed, Space.World);
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

	public override void Trigger()
	{
		moving = true;
		Drop_Object drop = GetComponent<Drop_Object>();
		if (drop != null)
		{
			drop.enabled = true;
		}
	}
}
