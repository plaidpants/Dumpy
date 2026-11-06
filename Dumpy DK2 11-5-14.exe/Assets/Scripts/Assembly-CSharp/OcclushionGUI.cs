using UnityEngine;

public class OcclushionGUI : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		BoxCollider bc = GetComponent<BoxCollider>();
		if (bc != null)
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
			Gizmos.DrawCube(Vector3.zero, bc.size);
		}
	}

	private void OnDrawGizmosSelected()
	{
		BoxCollider bc = GetComponent<BoxCollider>();
		if (bc != null)
		{
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Gizmos.color = new Color(0.5f, 1f, 0f, 0.5f);
			Gizmos.DrawCube(Vector3.zero, bc.size);
		}
	}
}
