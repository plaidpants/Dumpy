using UnityEngine;

public class MeshRenderOn : MonoBehaviour
{
	private void Start()
	{
		MeshRenderer mr = GetComponent<MeshRenderer>();
		if (mr != null)
		{
			mr.enabled = true;
		}
	}
}
