using UnityEngine;

public class Lettering : Actor
{
	private new void Start()
	{
		base.Start();
		if (rig != null)
		{
			rig.AddForce(new Vector3(Random.Range(10, 50), Random.Range(10, 50), Random.Range(10, 50)));
		}
	}

	public override void Trigger()
	{
	}
}
