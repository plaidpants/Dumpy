using UnityEngine;

public class DumpyBall : Actor
{
	private Transform player;
	private bool triggered;
	public float force;

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
	}

	private void FixedUpdate()
	{
		if (triggered && player != null && rig != null)
		{
			rig.AddForce((player.position - transform.position).normalized * force);
			force += 0.1f;
		}
	}

	public override void Trigger()
	{
		triggered = true;
	}
}
