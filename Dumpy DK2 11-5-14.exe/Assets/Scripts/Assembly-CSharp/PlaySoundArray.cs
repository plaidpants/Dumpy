using UnityEngine;

public class PlaySoundArray : Actor
{
	public AudioSource[] soundClip;
	public bool loop;

	public override void Trigger()
	{
		if (soundClip != null)
		{
			for (int i = 0; i < soundClip.Length; i++)
			{
				if (soundClip[i] != null)
				{
					soundClip[i].Play();
					if (loop)
					{
						soundClip[i].loop = true;
					}
				}
			}
		}
	}
}
