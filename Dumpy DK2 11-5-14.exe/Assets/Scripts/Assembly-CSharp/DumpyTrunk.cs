using UnityEngine;

public class DumpyTrunk : MonoBehaviour
{
	public Transform headAnchor;
	// Root segment sits 2.0m below and 1.5m behind eye level so it never blocks the camera view
	public Vector3 localOffset = new Vector3(0f, -2.0f, -1.5f);
	// -90 degrees on X lays the capsule flat so its local -Y axis points forward (+Z) and +Y points backward (-Z)
	public Vector3 localEuler = new Vector3(-90f, 0f, 0f);

	private GameObject[] segments = new GameObject[10];
	private Rigidbody[] segmentRbs = new Rigidbody[10];
	private bool isInitialized = false;
	private Vector3 lastHeadPos;
	private bool hasLastHeadPos = false;

	private void Awake()
	{
		int trunkLayer = LayerMask.NameToLayer("Trunk");
		if (trunkLayer < 0) trunkLayer = 8;
		// Original 2014 PhysicsManager disabled Layer 8 (Trunk) self-collision
		Physics.IgnoreLayerCollision(trunkLayer, trunkLayer, true);
		Physics.autoSyncTransforms = true;
	}

	private void Start()
	{
		InitializeTrunk();
	}

	public void InitializeTrunk()
	{
		if (isInitialized) return;

		FindHeadAnchor();

		for (int i = 1; i <= 10; i++)
		{
			segments[i - 1] = GameObject.Find("segment" + i.ToString("D2"));
		}

		if (segments[0] == null) return;

		int trunkLayer = LayerMask.NameToLayer("Trunk");
		if (trunkLayer < 0) trunkLayer = 8;
		Physics.IgnoreLayerCollision(trunkLayer, trunkLayer, true);

		// Unparent trunk container and segment01 so parent hierarchy transforms don't fight physics
		GameObject trunkGo = GameObject.Find("trunk");
		if (trunkGo != null)
		{
			trunkGo.transform.parent = null;
			trunkGo.transform.position = Vector3.zero;
			trunkGo.transform.rotation = Quaternion.identity;
			trunkGo.transform.localScale = Vector3.one;
		}

		segments[0].transform.parent = null;
		segments[0].layer = trunkLayer;
		segments[0].tag = "Trunk";

		Rigidbody rootRb = segments[0].GetComponent<Rigidbody>();
		if (rootRb == null) rootRb = segments[0].AddComponent<Rigidbody>();
		rootRb.mass = 1f;
		rootRb.drag = 1f;
		rootRb.angularDrag = 0.05f;
		rootRb.isKinematic = true;
		rootRb.useGravity = false;
		rootRb.interpolation = RigidbodyInterpolation.None;
		segmentRbs[0] = rootRb;

		// Original 2014 Rigidbody masses for segment02..segment10
		float[] masses = new float[] { 50f, 45f, 40f, 35f, 30f, 25f, 20f, 15f, 10f };
		float[] drags = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
		float[] angularDrags = new float[] { 0.1f, 0.1f, 0.1f, 0.1f, 10.0f, 10.0f, 10.0f, 10.0f, 10.0f };

		// Place all segments along their initial rest curve before attaching joints
		PositionSegmentsAtRest(false);

		for (int i = 1; i < 10; i++)
		{
			if (segments[i] == null || segments[i - 1] == null) continue;

			GameObject curr = segments[i];
			GameObject prev = segments[i - 1];

			curr.transform.parent = null;
			curr.layer = trunkLayer;
			curr.tag = "Trunk";

			Rigidbody currRb = curr.GetComponent<Rigidbody>();
			if (currRb == null) currRb = curr.AddComponent<Rigidbody>();
			Rigidbody prevRb = prev.GetComponent<Rigidbody>();

			currRb.isKinematic = false;
			currRb.useGravity = false;
			currRb.mass = masses[i - 1];
			currRb.drag = drags[i - 1];
			currRb.angularDrag = angularDrags[i - 1];
			currRb.interpolation = RigidbodyInterpolation.None;
			currRb.collisionDetectionMode = CollisionDetectionMode.Discrete;
			currRb.maxAngularVelocity = 7f;
			segmentRbs[i] = currRb;

			// Remove any existing joints before creating clean joint
			Joint[] oldJoints = curr.GetComponents<Joint>();
			foreach (Joint j in oldJoints)
			{
				DestroyImmediate(j);
			}

			ConfigurableJoint joint = curr.AddComponent<ConfigurableJoint>();
			joint.connectedBody = prevRb;
			// Exact original 2014 binary anchor: (0, 1, 0) on curr (rear tip) connected to (0, 0, 0) on prev (center)
			joint.autoConfigureConnectedAnchor = false;
			joint.anchor = new Vector3(0f, 1f, 0f);
			joint.connectedAnchor = Vector3.zero;

			// Lock linear axes so capsules stay connected end-to-end with 50% overlap (2.0m center spacing)
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;

			// Free pitch (X) and yaw (Z) so segments never wedge or bounce against hard cone limits; lock axial twist (Y)
			joint.angularXMotion = ConfigurableJointMotion.Free;
			joint.angularYMotion = ConfigurableJointMotion.Locked;
			joint.angularZMotion = ConfigurableJointMotion.Free;

			// Gentle slerp drive aligns capsule orientation along the chain
			JointDrive drive = new JointDrive();
			drive.positionSpring = Mathf.Lerp(400f, 80f, (i - 1) / 8f);
			drive.positionDamper = Mathf.Lerp(80f, 20f, (i - 1) / 8f);
			drive.maximumForce = float.PositiveInfinity;
			joint.slerpDrive = drive;
			joint.rotationDriveMode = RotationDriveMode.Slerp;

			joint.projectionMode = JointProjectionMode.PositionAndRotation;
			joint.projectionDistance = 0.1f;
			joint.projectionAngle = 180f;
			joint.enableCollision = false;
			joint.enablePreprocessing = false;
		}

		// Explicitly ignore collisions between all trunk segments AND any colliders on OVRCameraRig / Dumpy
		Collider[] rigColliders = GetComponentsInParent<Collider>(true);
		Collider[] childRigColliders = GetComponentsInChildren<Collider>(true);

		for (int i = 0; i < 10; i++)
		{
			if (segments[i] == null) continue;
			Collider colI = segments[i].GetComponent<Collider>();
			if (colI == null) continue;

			for (int j = i + 1; j < 10; j++)
			{
				if (segments[j] == null) continue;
				Collider colJ = segments[j].GetComponent<Collider>();
				if (colJ != null)
				{
					Physics.IgnoreCollision(colI, colJ, true);
				}
			}

			if (rigColliders != null)
			{
				foreach (Collider rc in rigColliders)
				{
					if (rc != null && rc != colI)
					{
						Physics.IgnoreCollision(colI, rc, true);
					}
				}
			}

			if (childRigColliders != null)
			{
				foreach (Collider rc in childRigColliders)
				{
					if (rc != null && rc != colI)
					{
						Physics.IgnoreCollision(colI, rc, true);
					}
				}
			}
		}

		// Ensure DumpyShadow casts shadows without rendering error pink
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

		if (headAnchor != null)
		{
			lastHeadPos = headAnchor.position;
			hasLastHeadPos = true;
		}

		Physics.SyncTransforms();
		isInitialized = true;
	}

	private Vector3 GetRestSegmentDirection(int index)
	{
		// Gentle natural elephant trunk curve in headAnchor space (unit vector so chord length is exact 2.0m)
		// Dips slightly below eye level near the base, then levels out forward so it reaches 18.5m ahead
		float[] dipAnglesDeg = new float[] { 0f, 8f, 10f, 10f, 8f, 6f, 4f, 2f, 0f, -2f };
		float angleRad = dipAnglesDeg[Mathf.Clamp(index, 0, 9)] * Mathf.Deg2Rad;
		Vector3 localDir = new Vector3(0f, -Mathf.Sin(angleRad), Mathf.Cos(angleRad));
		return headAnchor != null ? headAnchor.TransformDirection(localDir).normalized : localDir.normalized;
	}

	private void PositionSegmentsAtRest(bool resetVelocities)
	{
		if (headAnchor == null || segments[0] == null) return;

		Vector3 pos = headAnchor.TransformPoint(localOffset);
		Quaternion baseRot = headAnchor.rotation * Quaternion.Euler(localEuler);

		segments[0].transform.position = pos;
		segments[0].transform.rotation = baseRot;
		if (segmentRbs[0] != null)
		{
			segmentRbs[0].position = pos;
			segmentRbs[0].rotation = baseRot;
		}

		for (int i = 1; i < 10; i++)
		{
			if (segments[i] == null) continue;
			Vector3 dir = GetRestSegmentDirection(i);
			pos += dir * 2.0f;
			Quaternion segRot = Quaternion.FromToRotation(Vector3.down, dir);

			segments[i].transform.position = pos;
			segments[i].transform.rotation = segRot;

			if (segmentRbs[i] != null)
			{
				segmentRbs[i].position = pos;
				segmentRbs[i].rotation = segRot;
				if (resetVelocities && !segmentRbs[i].isKinematic)
				{
					segmentRbs[i].velocity = Vector3.zero;
					segmentRbs[i].angularVelocity = Vector3.zero;
				}
			}
		}
		Physics.SyncTransforms();
	}

	public void TeleportToHead()
	{
		FindHeadAnchor();
		if (headAnchor == null) return;
		PositionSegmentsAtRest(true);
		lastHeadPos = headAnchor.position;
		hasLastHeadPos = true;
	}

	private void FindHeadAnchor()
	{
		if (headAnchor != null) return;
		GameObject centerEye = GameObject.Find("CenterEyeAnchor");
		if (centerEye != null)
		{
			headAnchor = centerEye.transform;
			return;
		}
		Camera mainCam = Camera.main;
		if (mainCam != null)
		{
			headAnchor = mainCam.transform;
		}
	}

	private void FixedUpdate()
	{
		if (!isInitialized)
		{
			InitializeTrunk();
			if (!isInitialized) return;
		}

		if (headAnchor == null)
		{
			FindHeadAnchor();
			if (headAnchor == null) return;
		}

		float dt = Time.fixedDeltaTime;
		if (dt <= 0.0001f) dt = 0.02f;

		// Detect large teleports (such as teleporting to (0, -500, 0) inside the spaceship at the end of Main_03)
		Vector3 currentHeadPos = headAnchor.position;
		Vector3 headVel = Vector3.zero;
		if (hasLastHeadPos)
		{
			Vector3 headDelta = currentHeadPos - lastHeadPos;
			if (headDelta.sqrMagnitude > 25f) // > 5m jump in a single physics step
			{
				TeleportToHead();
				return;
			}
			headVel = headDelta / dt;
		}
		lastHeadPos = currentHeadPos;
		hasLastHeadPos = true;

		// Move root segment (segment01) cleanly with headAnchor
		Vector3 rootTargetPos = headAnchor.TransformPoint(localOffset);
		Quaternion rootTargetRot = headAnchor.rotation * Quaternion.Euler(localEuler);

		if (segmentRbs[0] != null)
		{
			segmentRbs[0].MovePosition(rootTargetPos);
			segmentRbs[0].MoveRotation(rootTargetRot);
		}
		else if (segments[0] != null)
		{
			segments[0].transform.position = rootTargetPos;
			segments[0].transform.rotation = rootTargetRot;
		}

		// Compute ideal curved rest positions (spaced at exact 2.0m intervals matching the joints)
		// and drive each dynamic segment with an unconditionally stable whip-follow velocity curve
		Vector3 curvePos = rootTargetPos;
		for (int i = 1; i < 10; i++)
		{
			Rigidbody rb = segmentRbs[i];
			if (rb == null) continue;

			Vector3 dir = GetRestSegmentDirection(i);
			curvePos += dir * 2.0f;

			// Graded response rate: stiff/fast at base (segment02) so it never jitters, whippy at tip (segment10)
			float t = (i - 1) / 8f;
			float omega = Mathf.Lerp(16f, 6.5f, t);

			Vector3 posError = curvePos - rb.position;
			Vector3 desiredVel = headVel + posError * omega;

			// Cap recovery speed to prevent physics spikes after heavy collisions
			float maxSpeed = 45f;
			if (desiredVel.sqrMagnitude > maxSpeed * maxSpeed)
			{
				desiredVel = desiredVel.normalized * maxSpeed;
			}

			// Unconditionally stable exponential velocity blend (0 < alpha < 1 for all dt)
			float blendRate = Mathf.Lerp(18f, 8f, t);
			float alpha = 1f - Mathf.Exp(-blendRate * dt);
			rb.velocity = Vector3.Lerp(rb.velocity, desiredVel, alpha);

			// Dampen stray angular velocity so segments never twist or vibrate
			rb.angularVelocity *= Mathf.Exp(-14f * dt);
		}
	}
}
