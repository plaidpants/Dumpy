using UnityEngine;

public class LookAtTatget : MonoBehaviour
{
	public Transform target;

	private void Update()
	{
		if (target != null)
		{
			transform.LookAt(target);
		}
	}
}
