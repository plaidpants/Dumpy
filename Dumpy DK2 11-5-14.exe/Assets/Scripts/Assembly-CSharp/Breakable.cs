using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Breakable : MonoBehaviour
{
	public GameObject brokenObj;
	public GameObject particle;
	public float strength;
	private bool hasBeenHit;
	private GameObject breakSound;

	private void Awake()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody>();
		}
		// Non-convex MeshColliders with non-kinematic Rigidbodies violate PhysX rules since Unity 5
		MeshCollider mc = GetComponent<MeshCollider>();
		if (mc != null && !mc.convex)
		{
			rb.isKinematic = true;
		}

		GameObject soundContainer = GameObject.Find("SoundContainer");
		if (soundContainer != null)
		{
			SoundCrashs soundCrashs = soundContainer.GetComponent<SoundCrashs>();
			if (soundCrashs != null && soundCrashs.crashSound != null && soundCrashs.crashSound.Length > 0)
			{
				int num = Random.Range(0, soundCrashs.crashSound.Length);
				breakSound = soundCrashs.crashSound[num];
			}
		}
	}

	private void Update()
	{
		if (hasBeenHit)
		{
			if (brokenObj != null)
			{
				GameObject gameObject = Instantiate(brokenObj, transform.position, transform.rotation) as GameObject;
				gameObject.transform.localScale = transform.localScale;

				// Ensure all dynamic pieces have convex mesh colliders
				foreach (MeshCollider mc in gameObject.GetComponentsInChildren<MeshCollider>(true))
				{
					if (mc != null && mc.GetComponent<Rigidbody>() != null)
					{
						mc.convex = true;
					}
				}

				if (particle != null)
				{
					GameObject p = Instantiate(particle, transform.position, transform.rotation) as GameObject;
					p.transform.parent = gameObject.transform;
				}
				if (breakSound != null)
				{
					GameObject s = Instantiate(breakSound, transform.position, Quaternion.identity) as GameObject;
					s.transform.parent = gameObject.transform;
				}
			}
			Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.impulse.sqrMagnitude > strength)
		{
			hasBeenHit = true;
		}
	}
}
