using UnityEngine;

public class SetCamFar : MonoBehaviour
{
	private void Start()
	{
		Camera component = GetComponent<Camera>();
		if (component != null)
		{
			component.farClipPlane = 1000f;
		}
	}
}
