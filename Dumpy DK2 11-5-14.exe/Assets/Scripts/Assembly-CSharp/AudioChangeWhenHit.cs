using UnityEngine;

public class AudioChangeWhenHit : MonoBehaviour
{
	public AudioClip clipToBeInserted;
	public bool loop;
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		switch (LayerMask.LayerToName(collision.gameObject.layer))
		{
		case "Trunk":
			if (collision.relativeVelocity.sqrMagnitude > 300f && audioSource != null)
			{
				audioSource.clip = clipToBeInserted;
				audioSource.loop = loop;
				audioSource.Play();
			}
			break;
		}
	}
}
