using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
	private float timeToLive;
	private float timer;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > timeToLive)
		{
			Destroy(gameObject);
		}
	}

	public void setTime(float time)
	{
		timeToLive = time;
	}
}
