using UnityEngine;

public class TurnOnUFOGenerator : Actor
{
	private new void Start()
	{
		base.Start();
		UFOGenerator gen = GetComponent<UFOGenerator>();
		if (gen != null)
		{
			gen.enabled = false;
		}
	}

	public override void Trigger()
	{
		UFOGenerator gen = GetComponent<UFOGenerator>();
		if (gen != null)
		{
			gen.enabled = true;
		}
	}
}
