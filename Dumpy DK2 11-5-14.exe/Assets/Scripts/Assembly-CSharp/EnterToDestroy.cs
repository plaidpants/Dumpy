using UnityEngine;

public class EnterToDestroy : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider != null && collider.gameObject != null)
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("Trunk") ||
				collider.gameObject.name.Contains("segment") ||
				collider.gameObject.name.Contains("OVRCameraRig") ||
				collider.GetComponentInParent<OVRCameraRig>() != null)
			{
				return;
			}
			Destroy(collider.gameObject);
		}
	}
}
