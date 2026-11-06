using UnityEngine;

public class Cloud : MonoBehaviour
{
	public float speed;
	public float rotSpeed;

	private void Update()
	{
		transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
		transform.Translate(Vector3.right * speed * Time.deltaTime);
	}
}
