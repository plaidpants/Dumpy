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
		if (!GetComponent<Rigidbody>())
		{
			gameObject.AddComponent<Rigidbody>();
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
