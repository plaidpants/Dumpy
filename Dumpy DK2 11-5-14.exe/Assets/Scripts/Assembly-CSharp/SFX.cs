using UnityEngine;

public class SFX : MonoBehaviour
{
	private void Update()
	{
		AudioSource a = GetComponent<AudioSource>();
		if (a != null && !a.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
