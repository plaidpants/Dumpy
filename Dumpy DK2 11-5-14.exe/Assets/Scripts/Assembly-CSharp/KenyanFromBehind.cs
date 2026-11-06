using UnityEngine;

public class KenyanFromBehind : Actor
{
	private GameObject kenyanBoy;

	private new void Start()
	{
		base.Start();
		kenyanBoy = GameObject.Find("BoyRunning_Behind");
		if (kenyanBoy != null)
		{
			kenyanBoy.SetActive(false);
		}
	}

	public override void Trigger()
	{
		if (kenyanBoy != null)
		{
			kenyanBoy.SetActive(true);
			AudioSource audio = kenyanBoy.GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
		}
	}
}
