using UnityEngine;

public class EffecientDump : MonoBehaviour
{
	private void Start()
	{
		// Ensure all scene renderers are enabled throughout the track
		MeshRenderer[] renderers = Object.FindObjectsOfType<MeshRenderer>(true);
		foreach (MeshRenderer r in renderers)
		{
			if (r != null && r.gameObject.name != "DumpyShadow")
			{
				r.enabled = true;
			}
		}
	}
}
