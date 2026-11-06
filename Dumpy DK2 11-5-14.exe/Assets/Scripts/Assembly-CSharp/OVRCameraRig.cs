using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

[ExecuteInEditMode]
public class OVRCameraRig : MonoBehaviour
{
	public Transform leftEyeAnchor { get; private set; }
	public Transform centerEyeAnchor { get; private set; }
	public Transform rightEyeAnchor { get; private set; }

	public bool enableMouseLookFallback = true;
	public float mouseSensitivity = 0.3f;
	private float yaw = 0f;
	private float pitch = 0f;

	private void Awake()
	{
		EnsureGameObjectIntegrity();
	}

	private void Start()
	{
		EnsureGameObjectIntegrity();
		StartXRIfAvailable();
		EnsureTrunk();
		EnsureAndroidAudioFocus();
	}

	private void EnsureAndroidAudioFocus()
	{
		if (!Application.isPlaying) return;
		if (GetComponent<AndroidAudioFocusRequester>() == null && Object.FindObjectOfType<AndroidAudioFocusRequester>() == null)
		{
			gameObject.AddComponent<AndroidAudioFocusRequester>();
		}
	}

	private void EnsureTrunk()
	{
		if (!Application.isPlaying) return;
		if (GetComponent<DumpyTrunk>() == null)
		{
			gameObject.AddComponent<DumpyTrunk>();
		}
	}

	private void OnDestroy()
	{
		StopXR();
	}

	private void OnDisable()
	{
		StopXR();
	}

	private void OnApplicationQuit()
	{
		StopXR();
	}

	private void StopXR()
	{
		if (Application.isPlaying && XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
		{
			var manager = XRGeneralSettings.Instance.Manager;
			if (manager.isInitializationComplete)
			{
				if (manager.activeLoader != null)
				{
					manager.StopSubsystems();
				}
				manager.DeinitializeLoader();
			}
		}
	}

	private void StartXRIfAvailable()
	{
		if (!Application.isPlaying) return;

		if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
		{
			var manager = XRGeneralSettings.Instance.Manager;
			if (!manager.isInitializationComplete)
			{
				manager.InitializeLoaderSync();
			}
			if (manager.activeLoader != null)
			{
				manager.StartSubsystems();
			}
		}
	}

	private void FixedUpdate()
	{
		if (Application.isPlaying)
		{
			UpdateAnchors();
		}
	}

	private void LateUpdate()
	{
		EnsureGameObjectIntegrity();
		if (Application.isPlaying)
		{
			UpdateAnchors();
		}
	}

	private void UpdateAnchors()
	{
		bool tracked = false;

		List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);

		if (devices.Count > 0 && devices[0].isValid)
		{
			UnityEngine.XR.InputDevice head = devices[0];
			if (head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion rot))
			{
				if (centerEyeAnchor != null) centerEyeAnchor.localRotation = rot;
				if (leftEyeAnchor != null) leftEyeAnchor.localRotation = rot;
				if (rightEyeAnchor != null) rightEyeAnchor.localRotation = rot;
				tracked = true;
			}
			if (head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 pos))
			{
				if (centerEyeAnchor != null) centerEyeAnchor.localPosition = pos;
				if (leftEyeAnchor != null) leftEyeAnchor.localPosition = pos;
				if (rightEyeAnchor != null) rightEyeAnchor.localPosition = pos;
			}
		}

		if (!tracked && Application.isPlaying && enableMouseLookFallback)
		{
			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				if (mouse.leftButton.wasPressedThisFrame)
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
				Keyboard keyboard = Keyboard.current;
				if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
				{
					Cursor.lockState = CursorLockMode.None;
				}

				Vector2 delta = mouse.delta.ReadValue();
				float mouseX = delta.x * mouseSensitivity;
				float mouseY = delta.y * mouseSensitivity;
				yaw += mouseX;
				pitch -= mouseY;
				pitch = Mathf.Clamp(pitch, -85f, 85f);
				Quaternion mouseRot = Quaternion.Euler(pitch, yaw, 0f);

				if (centerEyeAnchor != null) centerEyeAnchor.localRotation = mouseRot;
				if (leftEyeAnchor != null) leftEyeAnchor.localRotation = mouseRot;
				if (rightEyeAnchor != null) rightEyeAnchor.localRotation = mouseRot;
			}
		}
	}

	private void EnsureGameObjectIntegrity()
	{
		if (centerEyeAnchor == null)
			centerEyeAnchor = ConfigureEyeAnchor("CenterEyeAnchor");
		if (leftEyeAnchor == null)
			leftEyeAnchor = ConfigureEyeAnchor("LeftEyeAnchor");
		if (rightEyeAnchor == null)
			rightEyeAnchor = ConfigureEyeAnchor("RightEyeAnchor");

		SetupStereoCamera();

		// Ensure DumpyShadow never renders as error pink
		GameObject shadow = GameObject.Find("DumpyShadow");
		if (shadow != null)
		{
			MeshRenderer smr = shadow.GetComponent<MeshRenderer>();
			if (smr != null)
			{
				smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				smr.receiveShadows = false;
			}
		}
	}

	private void SetupStereoCamera()
	{
		if (centerEyeAnchor == null) return;

		Camera centerCam = centerEyeAnchor.GetComponent<Camera>();
		Camera leftCam = leftEyeAnchor != null ? leftEyeAnchor.GetComponent<Camera>() : null;
		Camera rightCam = rightEyeAnchor != null ? rightEyeAnchor.GetComponent<Camera>() : null;

		if (centerCam == null)
		{
			centerCam = centerEyeAnchor.gameObject.AddComponent<Camera>();
		}

		// Configure centerCam as the primary stereo VR and desktop camera
		centerCam.stereoTargetEye = StereoTargetEyeMask.Both;
		centerCam.enabled = true;
		centerCam.tag = "MainCamera";

		if (leftCam != null)
		{
			centerCam.clearFlags = leftCam.clearFlags;
			centerCam.backgroundColor = leftCam.backgroundColor;
			centerCam.cullingMask = leftCam.cullingMask;
			centerCam.nearClipPlane = 0.1f;
			centerCam.farClipPlane = (leftCam.farClipPlane > 0f) ? leftCam.farClipPlane : 1000f;
			centerCam.depth = leftCam.depth;

			// Disable legacy separate eye cameras so they don't block or duplicate VR rendering
			if (Application.isPlaying)
			{
				leftCam.enabled = false;
			}
		}

		if (rightCam != null && Application.isPlaying)
		{
			rightCam.enabled = false;
		}

		// Ensure AudioListener is on center eye anchor
		if (centerEyeAnchor.GetComponent<AudioListener>() == null)
		{
			centerEyeAnchor.gameObject.AddComponent<AudioListener>();
		}

		if (Application.isPlaying)
		{
			if (leftEyeAnchor != null)
			{
				AudioListener al = leftEyeAnchor.GetComponent<AudioListener>();
				if (al != null) Destroy(al);
			}
			if (rightEyeAnchor != null)
			{
				AudioListener al = rightEyeAnchor.GetComponent<AudioListener>();
				if (al != null) Destroy(al);
			}
		}
	}

	private Transform ConfigureEyeAnchor(string name)
	{
		Transform child = transform.Find(name);
		if (child == null)
		{
			Transform trackingSpace = transform.Find("TrackingSpace");
			if (trackingSpace != null)
			{
				child = trackingSpace.Find(name);
			}
		}
		if (child == null)
		{
			GameObject go = new GameObject(name);
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			child = go.transform;
		}
		return child;
	}
}
