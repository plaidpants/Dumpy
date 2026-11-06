using UnityEngine;

public class Runner : Actor
{
	public bool startTriggered = true;
	public float animationSpeed = 1f;
	public float speed = 1.75f;
	public float lookSpeed = 2f;
	public float moveDist = 20f;
	public float minDist = 3f;
	public float hitVelocity = 150f;
	public float tipPercent = 0.2f;
	public float timeToExplode = 1f;
	public GameObject brokenObj;
	public GameObject breakSound;
	public GameObject runParticle;
	public GameObject hitParticle;
	public GameObject yell;
	private bool moving;
	private bool hit;
	private GameObject runP;
	private GameObject hitP;
	private Transform player;
	private GameObject soundObj;
	private float timeInAir;
	private bool triggered;
	private float timeToDrop = 6f;
	private float dropTime;
	private bool droping;
	private Vector3 startPos;

	public override void Trigger()
	{
		triggered = true;
	}

	private new void Start()
	{
		base.Start();
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
		if (ani != null)
		{
			ani.speed = animationSpeed;
		}
		if (startTriggered)
		{
			Trigger();
		}
		startPos = transform.position;
	}

	private void Update()
	{
		if (player == null) return;
		Vector3 vector = new Vector3(player.position.x, transform.position.y, player.position.z);
		float num = Vector3.Distance(transform.position, vector);
		if (droping)
		{
			dropTime += Time.deltaTime;
			BoxCollider bc = GetComponent<BoxCollider>();
			if (dropTime > timeToDrop && bc != null)
			{
				Destroy(bc);
			}
		}
		if (transform.position.y < startPos.y - 100f)
		{
			Destroy(gameObject);
		}
		if (!hit)
		{
			TestForHit();
		}
		if (num < moveDist && !hit && !moving && triggered)
		{
			StartMove();
		}
		if (moving && hit)
		{
			StopMove();
		}
		if (moving)
		{
			Quaternion to = Quaternion.LookRotation(transform.position - vector);
			transform.rotation = Quaternion.Slerp(transform.rotation, to, Time.deltaTime * lookSpeed);
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
		}
		if (!hit)
		{
			return;
		}
		timeInAir += Time.deltaTime;
		if (timeInAir > timeToExplode)
		{
			if (brokenObj != null)
			{
				Instantiate(brokenObj, transform.position, transform.rotation);
			}
			Destroy(gameObject);
		}
	}

	private void StartMove()
	{
		moving = true;
		if (ani != null)
		{
			ani.SetBool("Run", true);
		}
		if (runParticle != null)
		{
			runP = Instantiate(runParticle, transform.position, Quaternion.identity) as GameObject;
			runP.transform.parent = transform;
		}
		if (yell != null)
		{
			GameObject gameObject = Instantiate(yell, transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = transform;
		}
	}

	private void StopMove()
	{
		moving = false;
		if (ani != null)
		{
			ani.SetBool("Run", false);
		}
		if (runP != null)
		{
			Destroy(runP);
		}
	}

	private void TestForHit()
	{
		if (rig != null && rig.velocity.magnitude > hitVelocity)
		{
			Hit();
		}
	}

	private void Hit()
	{
		hit = true;
		if (ani != null)
		{
			ani.SetBool("Hit", true);
		}
		if (hitParticle != null)
		{
			hitP = Instantiate(hitParticle, transform.position, Quaternion.identity) as GameObject;
			hitP.transform.parent = transform;
		}
		if (breakSound != null)
		{
			soundObj = Instantiate(breakSound, transform.position, Quaternion.identity) as GameObject;
			soundObj.transform.parent = transform;
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.collider == null || col.collider.attachedRigidbody == null)
		{
			return;
		}
		if (LayerMask.LayerToName(col.gameObject.layer) == "Trunk")
		{
			Vector3 velocity = col.collider.attachedRigidbody.velocity;
			if (rig != null)
			{
				rig.AddForce(velocity * 350f);
			}
			Hit();
		}
	}
}
