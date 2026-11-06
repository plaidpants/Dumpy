using UnityEngine;

public class Shard : MonoBehaviour
{
	private float strength = 450f;
	public GameObject hitParticle;
	private GameObject soundObj;
	private GameObject hitObj;
	private GameObject breakSound;
	private float theTime;
	private float sleepTime = 7f;
	private bool stoped;
	private Vector3 startPos;

	private void Start()
	{
		MeshRenderer mr = GetComponent<MeshRenderer>();
		if (mr != null) mr.enabled = true;
		startPos = transform.position;
		GameObject soundContainer = GameObject.Find("SoundContainer");
		if (soundContainer != null)
		{
			ShardSounds ss = soundContainer.GetComponent<ShardSounds>();
			if (ss != null && ss.shardSounds != null && ss.shardSounds.Length > 0)
			{
				int num = Random.Range(0, ss.shardSounds.Length);
				breakSound = ss.shardSounds[num];
			}
		}
	}

	private void Update()
	{
		theTime += Time.deltaTime;
		if (theTime > sleepTime && !stoped)
		{
			MeshCollider mc = GetComponent<MeshCollider>();
			if (mc != null) Destroy(mc);
			stoped = true;
		}
		if (transform.position.y < startPos.y - 200f)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.impulse.sqrMagnitude > strength)
		{
			if (soundObj == null && breakSound != null)
			{
				soundObj = Instantiate(breakSound, transform.position, Quaternion.identity) as GameObject;
				soundObj.transform.parent = transform;
			}
			if (hitObj == null && hitParticle != null)
			{
				hitObj = Instantiate(hitParticle, transform.position, Quaternion.identity) as GameObject;
				hitObj.transform.parent = transform;
			}
		}
	}
}
