using UnityEngine;

public class UFO_Beam : MonoBehaviour
{
	public float force = 1f;
	public GameObject[] beams;
	private Transform parent;
	private int rigsInBeam;

	private void Start()
	{
		parent = transform.parent;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other != null && other.GetComponent<Rigidbody>() != null)
		{
			rigsInBeam++;
			if (beams != null)
			{
				for (int i = 0; i < beams.Length; i++)
				{
					if (beams[i] != null) beams[i].SetActive(true);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other != null && other.GetComponent<Rigidbody>() != null)
		{
			rigsInBeam--;
		}
		if (rigsInBeam <= 0)
		{
			rigsInBeam = 0;
			if (beams != null)
			{
				for (int i = 0; i < beams.Length; i++)
				{
					if (beams[i] != null) beams[i].SetActive(false);
				}
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other != null && other.GetComponent<Rigidbody>() != null && parent != null)
		{
			Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddForce((parent.position - other.transform.position).normalized * force, ForceMode.Impulse);
			}
		}
	}
}
