using UnityEngine;

public class HitSound : MonoBehaviour
{
	public GameObject hitSound;
	private GameObject soundObj;

	private void OnCollisionEnter(Collision otherObject)
	{
		if (otherObject.collider != null && otherObject.collider.attachedRigidbody != null &&
			LayerMask.LayerToName(otherObject.gameObject.layer) == "Trunk" && soundObj == null && hitSound != null)
		{
			soundObj = Instantiate(hitSound, transform.position, Quaternion.identity) as GameObject;
			soundObj.transform.parent = transform;
		}
	}
}
