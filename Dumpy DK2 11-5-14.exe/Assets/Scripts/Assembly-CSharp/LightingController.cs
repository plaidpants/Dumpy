using UnityEngine;

public class LightingController : MonoBehaviour
{
	public Color StartLightColor;
	public Color EndLightColor;
	public Color StartAmbient;
	public Color EndAmbient;
	public Color StartFogColor;
	public Color EndFogColor;
	public float StartIntensity;
	public float EndIntensity;
	public float ChangeTime;
	public Light DirectionalLight;
	private float startTime;

	private void Start()
	{
		RenderSettings.fogStartDistance = 400f;
		RenderSettings.fogEndDistance = 850f;
		startTime = Time.time;
		GameObject dirLightGo = GameObject.Find("Directional light");
		if (dirLightGo != null)
		{
			DirectionalLight = dirLightGo.GetComponent<Light>();
		}
		RenderSettings.fogColor = StartFogColor;
		RenderSettings.ambientLight = StartAmbient;
		if (DirectionalLight != null)
		{
			DirectionalLight.color = StartLightColor;
			DirectionalLight.intensity = StartIntensity;
		}
	}

	private void Update()
	{
		if (ChangeTime <= 0f) return;
		float t = (Time.time - startTime) / ChangeTime;
		RenderSettings.fogColor = Color.Lerp(StartFogColor, EndFogColor, t);
		RenderSettings.ambientLight = Color.Lerp(StartAmbient, EndAmbient, t);
		if (DirectionalLight != null)
		{
			DirectionalLight.color = Color.Lerp(StartLightColor, EndLightColor, t);
			DirectionalLight.intensity = Mathf.Lerp(StartIntensity, EndIntensity, t);
		}
	}
}
