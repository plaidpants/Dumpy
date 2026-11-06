using UnityEngine;

public class lookAtDumpy : MonoBehaviour
{
	private float lookSpeed = 10f;
	private Transform player;

	private void Start()
	{
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

	private void Update()
	{
		if (player != null)
		{
			Vector3 dir = player.position - transform.position;
			if (dir.sqrMagnitude > 0.001f)
			{
				Quaternion to = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(transform.rotation, to, Time.deltaTime * lookSpeed);
			}
		}
	}
}
