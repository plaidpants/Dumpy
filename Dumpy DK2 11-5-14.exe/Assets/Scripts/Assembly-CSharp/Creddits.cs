using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Creddits : MonoBehaviour
{
	private float minDisplayTime = 1.0f; // Brief grace period so releasing a button from gameplay doesn't immediately skip
	private float timer = 0f;

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer < minDisplayTime) return;

		bool buttonPressed = false;

		// 1. Any button on any New Input System device (XR controllers, Gamepad, Keyboard, Mouse, Touch)
		foreach (var device in UnityEngine.InputSystem.InputSystem.devices)
		{
			foreach (var control in device.allControls)
			{
				if (control is ButtonControl button && button.wasPressedThisFrame)
				{
					buttonPressed = true;
					break;
				}
			}
			if (buttonPressed) break;
		}

		// 2. Legacy XR.InputDevices fallback for OpenXR controller buttons
		if (!buttonPressed)
		{
			var xrDevices = new List<UnityEngine.XR.InputDevice>();
			UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
				InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, xrDevices);
			foreach (var dev in xrDevices)
			{
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool p1) && p1) { buttonPressed = true; break; }
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool p2) && p2) { buttonPressed = true; break; }
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool tr) && tr) { buttonPressed = true; break; }
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool gr) && gr) { buttonPressed = true; break; }
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out bool mb) && mb) { buttonPressed = true; break; }
				if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool js) && js) { buttonPressed = true; break; }
			}
		}

		if (buttonPressed)
		{
			// Reset back to start of the experience
			SceneManager.LoadScene(0);
		}
	}
}
