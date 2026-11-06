using UnityEngine;

public class Partical : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (ps != null && !ps.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
