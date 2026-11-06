using UnityEngine;

public class TrunkJoint : MonoBehaviour
{
	public Transform segment;

	private void Update()
	{
		if (segment != null)
		{
			transform.position = segment.position;
		}
	}
}
