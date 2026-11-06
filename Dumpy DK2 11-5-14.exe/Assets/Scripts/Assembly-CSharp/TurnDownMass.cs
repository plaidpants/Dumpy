using UnityEngine;

public class TurnDownMass : MonoBehaviour
{
	public float distanceToTurnOff;
	public float endMass;
	public Transform objectDistanceFrom;
	private Rigidbody rg;

	private void Start()
	{
		rg = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (objectDistanceFrom != null && rg != null)
		{
			float num = Vector3.Distance(objectDistanceFrom.position, transform.position);
			if (num <= distanceToTurnOff)
			{
				rg.mass = endMass;
			}
		}
	}
}
