using UnityEngine;

public class Bumper : MonoBehaviour
{
	public float bumperPush;
	public bool pushEverything;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody rigidbody = null;
		if (!pushEverything)
		{
			switch (collider.gameObject.name)
			{
			case "RingLeader":
				Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.velocity *= bumperPush;
				}
				break;
			}
		}
		else
		{
			rigidbody = collider.gameObject.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.AddForce(0f - bumperPush, 0f, 0f);
			}
		}
	}
}
