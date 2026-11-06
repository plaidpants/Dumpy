using UnityEngine;

public class AndroidAudioFocusRequester : MonoBehaviour
{
    private static AndroidAudioFocusRequester instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RequestAndroidAudioFocus();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            RequestAndroidAudioFocus();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            RequestAndroidAudioFocus();
        }
    }

    private float timer = 0f;
    private int retryCount = 0;

    private void Update()
    {
        // Retry a few times in early startup frames to guarantee Horizon OS grants focus
        if (retryCount < 5)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= 0.5f)
            {
                timer = 0f;
                retryCount++;
                RequestAndroidAudioFocus();
            }
        }
    }

    public static void RequestAndroidAudioFocus()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                if (currentActivity == null) return;

                using (var audioManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "audio"))
                {
                    if (audioManager == null) return;

                    using (var versionClass = new AndroidJavaClass("android.os.Build$VERSION"))
                    {
                        int sdkInt = versionClass.GetStatic<int>("SDK_INT");
                        // Android 8.0+ (API 26+) / Horizon OS
                        if (sdkInt >= 26)
                        {
                            using (var audioAttributesBuilder = new AndroidJavaObject("android.media.AudioAttributes$Builder"))
                            {
                                // USAGE_MEDIA = 1, CONTENT_TYPE_MUSIC = 2
                                audioAttributesBuilder.Call<AndroidJavaObject>("setUsage", 1);
                                audioAttributesBuilder.Call<AndroidJavaObject>("setContentType", 2);
                                using (var audioAttributes = audioAttributesBuilder.Call<AndroidJavaObject>("build"))
                                using (var focusRequestBuilder = new AndroidJavaObject("android.media.AudioFocusRequest$Builder", 1)) // AUDIOFOCUS_GAIN = 1
                                {
                                    focusRequestBuilder.Call<AndroidJavaObject>("setAudioAttributes", audioAttributes);
                                    focusRequestBuilder.Call<AndroidJavaObject>("setAcceptsDelayedFocusGain", true);
                                    using (var focusRequest = focusRequestBuilder.Call<AndroidJavaObject>("build"))
                                    {
                                        int result = audioManager.Call<int>("requestAudioFocus", focusRequest);
                                        Debug.Log("[AndroidAudioFocusRequester] requestAudioFocus (API 26+) returned: " + result);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // STREAM_MUSIC = 3, AUDIOFOCUS_GAIN = 1
                            int result = audioManager.Call<int>("requestAudioFocus", null, 3, 1);
                            Debug.Log("[AndroidAudioFocusRequester] requestAudioFocus legacy returned: " + result);
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[AndroidAudioFocusRequester] Exception requesting audio focus: " + ex.Message);
        }
#endif
    }
}
