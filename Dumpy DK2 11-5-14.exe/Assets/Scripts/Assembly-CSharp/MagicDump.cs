using UnityEngine;

public class MagicDump : Actor
{
	private bool triggered;
	private float timeToAni = 5f;
	private float aniTime;
	private UVmover[] uvms;

	private new void Start()
	{
		base.Start();
		if (ani != null)
		{
			ani.speed = 0f;
		}
		uvms = GetComponentsInChildren<UVmover>();
	}

	public override void Trigger()
	{
		Debug.Log("HIT");
		triggered = true;
	}

	private void Update()
	{
		if (!triggered) return;
		aniTime += Time.deltaTime;
		if (aniTime > timeToAni)
		{
			if (uvms != null)
			{
				for (int i = 0; i < uvms.Length; i++)
				{
					if (uvms[i] != null) uvms[i].moving = true;
				}
			}
			if (ani != null)
			{
				ani.speed = 0.4f;
			}
			triggered = false;
		}
	}
}
