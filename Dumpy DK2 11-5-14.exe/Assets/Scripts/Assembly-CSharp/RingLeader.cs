using UnityEngine;

public class RingLeader : Actor
{
	public GameObject hitSound;
	public float speed;
	public bool startTriggered;
	private float currentSpeed;
	private Transform player;
	private GameObject soundObj;

	private new void Start()
	{
		base.Start();
		GameObject camRig = GameObject.Find("OVRCameraRig");
		if (camRig == null) camRig = GameObject.Find("OVRCameraRig NEW");
		if (camRig == null)
		{
			OVRCameraRig rig = Object.FindObjectOfType<OVRCameraRig>();
			if (rig != null) camRig = rig.gameObject;
		}
		if (camRig != null)
		{
			player = camRig.transform;
		}
		if (startTriggered)
		{
			Trigger();
		}
	}

	public override void Trigger()
	{
		currentSpeed = speed;
	}

	private void FixedUpdate()
	{
		if (player != null && rig != null)
		{
			Vector3 vector = transform.InverseTransformDirection(player.position - transform.position);
			rig.AddRelativeForce(vector.normalized * currentSpeed);
		}
	}

	private void OnCollisionEnter(Collision otherObject)
	{
		if (otherObject.collider == null || otherObject.collider.attachedRigidbody == null)
		{
			return;
		}
		switch (LayerMask.LayerToName(otherObject.gameObject.layer))
		{
		case "Trunk":
			currentSpeed = 0f;
			Vector3 velocity = otherObject.collider.attachedRigidbody.velocity;
			if (rig != null)
			{
				rig.AddForce(velocity * 350f);
			}
			if (soundObj == null && hitSound != null)
			{
				soundObj = Instantiate(hitSound, transform.position, Quaternion.identity) as GameObject;
				soundObj.transform.parent = transform;
			}
			break;
		}
	}
}
