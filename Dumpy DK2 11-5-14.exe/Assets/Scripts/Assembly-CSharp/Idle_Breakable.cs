using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Idle_Breakable : MonoBehaviour
{
	private Rigidbody rigidBody;
	public bool isBuilding;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody>();
		MeshCollider mc = GetComponent<MeshCollider>();
		if (rigidBody != null && mc != null && !mc.convex)
		{
			rigidBody.isKinematic = true;
		}
	}

	private void Start()
	{
		if (rigidBody == null) rigidBody = GetComponent<Rigidbody>();
		if (rigidBody != null)
		{
			rigidBody.useGravity = false;
		}
	}

	private void OnCollisionEnter(Collision collider)
	{
		if (!isBuilding)
		{
			Animator anim = GetComponent<Animator>();
			if (anim != null)
			{
				anim.speed = 0f;
			}
			if (rigidBody != null)
			{
				MeshCollider mc = GetComponent<MeshCollider>();
				if (mc == null || mc.convex)
				{
					rigidBody.isKinematic = false;
					rigidBody.useGravity = true;
				}
			}
		}
		else
		{
			string text = LayerMask.LayerToName(collider.gameObject.layer);
			if (text != "Building" && text != "Ground")
			{
				if (rigidBody != null)
				{
					MeshCollider mc = GetComponent<MeshCollider>();
					if (mc == null || mc.convex)
					{
						rigidBody.isKinematic = false;
						rigidBody.useGravity = true;
					}
				}
			}
		}
	}
}
