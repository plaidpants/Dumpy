using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour
{
	private void Awake()
	{
		BoxCollider bc = GetComponent<BoxCollider>();
		if (bc != null)
		{
			bc.isTrigger = true;
		}
	}

	private void TriggerEnter()
	{
		Debug.Log(transform.name + ": Trigger Enter");
		Actor[] actors = Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		if (actors != null)
		{
			foreach (Actor actor in actors)
			{
				if (actor != null && actor.triggerName == gameObject.name)
				{
					actor.Trigger();
				}
			}
		}
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col != null && (col.gameObject.name.Contains("OVRCameraRig") || col.GetComponentInParent<OVRCameraRig>() != null))
		{
			TriggerEnter();
		}
	}

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
