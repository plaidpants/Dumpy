using UnityEngine;

public class TurnOnHighwaySystem : Actor
{
	private new void Start()
	{
		base.Start();
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public override void Trigger()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}
}
