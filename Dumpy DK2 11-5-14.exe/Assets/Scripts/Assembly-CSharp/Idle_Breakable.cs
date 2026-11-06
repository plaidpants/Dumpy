using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Idle_Breakable : MonoBehaviour
{
	private Rigidbody rigidBody;
	public bool isBuilding;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
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
				rigidBody.useGravity = true;
			}
		}
		else
		{
			string text = LayerMask.LayerToName(collider.gameObject.layer);
			if (text != "Building" && text != "Ground")
			{
				if (rigidBody != null)
				{
					rigidBody.useGravity = true;
				}
			}
		}
	}
}
