using UnityEngine;

public class MeshRenderOn : MonoBehaviour
{
	private void Awake()
	{
		// Ensure MeshColliders on dynamic objects (such as the donut on top of DonutTruck) are convex so they collide
		MeshCollider mc = GetComponent<MeshCollider>();
		if (mc != null && GetComponentInParent<Rigidbody>() != null)
		{
			mc.convex = true;
		}
	}

	private void Start()
	{
		MeshRenderer mr = GetComponent<MeshRenderer>();
		if (mr != null)
		{
			mr.enabled = true;
		}
	}
}
