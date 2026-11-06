using UnityEngine;

public class PlaySound : Actor
{
	public AudioSource soundClip;
	public bool loop;

	public override void Trigger()
	{
		if (soundClip != null)
		{
			soundClip.Play();
			if (loop)
			{
				soundClip.loop = true;
			}
		}
	}
}
