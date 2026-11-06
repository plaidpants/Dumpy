using UnityEngine;

public class OVRManager : MonoBehaviour
{
	public float nativeTextureScale = 1f;
	public float virtualTextureScale = 1f;
	public bool usePositionTracking = true;
	public RenderTextureFormat eyeTextureFormat = RenderTextureFormat.Default;
	public int eyeTextureDepth = 24;
	public bool timeWarp = true;
	public bool freezeTimeWarp;
	public bool resetTrackerOnLoad = true;
	public bool monoscopic;

	public static OVRManager instance { get; private set; }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}
		instance = this;
	}
}
