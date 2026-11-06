using UnityEngine;

public class Plane : MonoBehaviour
{
	public float strength = 50f;
	public GameObject brokenObj;
	public GameObject particle;
	public GameObject breakSound;
	public float speed;
	private float turnSpeed = 90f;
	private float turnTime = 2f;
	private float theTime;
	public bool turrning;
	private float startOfTurn;
	private bool hit;

	private void Update()
	{
		if (!hit)
		{
			transform.Translate(Vector3.forward * Time.deltaTime * speed);
			theTime += Time.deltaTime;
			if (theTime > turnTime)
			{
				theTime = 0f;
				startOfTurn = transform.rotation.y;
				turrning = !turrning;
			}
			if (turrning)
			{
				transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		hit = true;
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.useGravity = true;
		}
		if (col.impulse.sqrMagnitude > strength)
		{
			if (particle != null)
			{
				Instantiate(particle, transform.position, transform.rotation);
			}
			if (breakSound != null)
			{
				Instantiate(breakSound, transform.position, Quaternion.identity);
			}
			if (brokenObj != null)
			{
				GameObject gameObject = Instantiate(brokenObj, transform.position, transform.rotation) as GameObject;
				gameObject.transform.localScale = transform.localScale;
				Destroy(base.gameObject);
			}
		}
	}
}
