using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Creddits : MonoBehaviour
{
	private float minDisplayTime = 2.5f; // Grace period before accepting restart input
	private float timer = 0f;
	private bool prevLegacyButtonPressed = false;

	private void Update()
	{
		timer += Time.deltaTime;

		bool buttonPressed = false;
		bool anyLegacyButtonDown = false;

		// 1. New Input System: Handheld controllers, gamepads, keyboard, mouse
		foreach (var device in UnityEngine.InputSystem.InputSystem.devices)
		{
			// Explicitly ignore HMD and head trackers so head motion or userPresence never auto-triggers restart
			if (device is UnityEngine.InputSystem.XR.XRHMD) continue;
			if (device.description.deviceClass == "HMD") continue;

			if (device is UnityEngine.InputSystem.XR.XRController xrCtrl)
			{
				// Check discrete action buttons only (ignore capacitive touch / proximity controls)
				string[] actionButtonNames = { "primaryButton", "secondaryButton", "menuButton", "thumbstickClicked", "primary2DAxisClick", "triggerButton" };
				foreach (var btnName in actionButtonNames)
				{
					if (xrCtrl.TryGetChildControl<ButtonControl>(btnName) is ButtonControl btn && btn.wasPressedThisFrame)
					{
						buttonPressed = true;
						break;
					}
				}
			}
			else if (device is Gamepad gamepad)
			{
				if (gamepad.buttonSouth.wasPressedThisFrame ||
				    gamepad.buttonEast.wasPressedThisFrame ||
				    gamepad.buttonWest.wasPressedThisFrame ||
				    gamepad.buttonNorth.wasPressedThisFrame ||
				    gamepad.startButton.wasPressedThisFrame)
				{
					buttonPressed = true;
				}
			}
			else if (device is Keyboard keyboard)
			{
				if (keyboard.spaceKey.wasPressedThisFrame ||
				    keyboard.enterKey.wasPressedThisFrame ||
				    keyboard.anyKey.wasPressedThisFrame)
				{
					buttonPressed = true;
				}
			}
			else if (device is Mouse mouse)
			{
				if (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)
				{
					buttonPressed = true;
				}
			}

			if (buttonPressed) break;
		}

		// 2. Legacy XR.InputDevices fallback strictly for handheld controllers
		var xrDevices = new List<UnityEngine.XR.InputDevice>();
		UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
			InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, xrDevices);
		foreach (var dev in xrDevices)
		{
			if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool p1) && p1) anyLegacyButtonDown = true;
			if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool p2) && p2) anyLegacyButtonDown = true;
			if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out bool mb) && mb) anyLegacyButtonDown = true;
			if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool js) && js) anyLegacyButtonDown = true;
			if (dev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool tr) && tr) anyLegacyButtonDown = true;
		}

		// Enforce edge-triggering for legacy XR inputs
		if (anyLegacyButtonDown && !prevLegacyButtonPressed)
		{
			buttonPressed = true;
		}
		prevLegacyButtonPressed = anyLegacyButtonDown;

		// Only trigger restart if the grace period has elapsed AND a discrete button press occurred
		if (timer >= minDisplayTime && buttonPressed)
		{
			// Purge unused memory and assets from the previous run to prevent GC hitches upon restarting
			System.GC.Collect();
			Resources.UnloadUnusedAssets();

			// Reset back to start of the experience
			SceneManager.LoadScene(0);
		}
	}
}
