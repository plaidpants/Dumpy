using UnityEngine;

public class ExplodeHouse : Actor
{
	private new void Start()
	{
		base.Start();
		if (rig != null)
		{
			rig.AddForce(new Vector3(Random.Range(1000, 2000), Random.Range(1000, 2000), Random.Range(1000, 2000)));
		}
	}

	public override void Trigger()
	{
	}
}
