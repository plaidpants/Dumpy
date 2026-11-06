using UnityEngine;

public class Pinball : MonoBehaviour
{
	private void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject != null && otherObject.gameObject.name == "RingLeader")
		{
			Debug.Log("HIT");
			AudioSource audio = GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
		}
	}
}
