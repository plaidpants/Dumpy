using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BoyP : MonoBehaviour
{
	public GameObject particle;
	public float aniSpeed;
	public Transform endPos;
	public GUIStyle guiStyle;
	private Animator ani;
	private float timeToStop = 9.5f;
	private float theTime;
	private Color color1;
	private Color color2;
	private bool moveTonextScene;
	private float timeToMove = 6f;
	private float moveTime;
	private float timeToExplode = 1.2f;
	private float explodeTime;
	private bool canExplode;

	private Transform plungerTarget;
	private Transform boyTarget;
	private Transform[] trunkSegments;

	private void Start()
	{
		color1 = new Color(1f, 1f, 1f, 1f);
		color2 = new Color(1f, 1f, 1f, 0f);
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		if (agent != null)
		{
			if (agent.isOnNavMesh && endPos != null)
			{
				agent.SetDestination(endPos.position);
			}
			else
			{
				agent.enabled = false;
			}
		}
		ani = GetComponent<Animator>();
		if (ani != null)
		{
			ani.speed = aniSpeed;
		}

		// Ensure colliders cover both the Plunger (at local z ~ 6.06) and the Boy (at local z ~ 0)
		BoxCollider existingBc = GetComponent<BoxCollider>();
		if (existingBc != null)
		{
			existingBc.size = new Vector3(5f, 7f, 5f);
		}
		BoxCollider boyBc = gameObject.AddComponent<BoxCollider>();
		boyBc.center = new Vector3(0f, 3.5f, 0f);
		boyBc.size = new Vector3(4.5f, 7f, 4.5f);

		plungerTarget = transform.Find("joint16");
		boyTarget = transform.Find("joint1");
	}

	private void CheckTrunkProximity()
	{
		if (moveTonextScene || theTime < 4f) return;

		if (trunkSegments == null || trunkSegments[0] == null)
		{
			trunkSegments = new Transform[10];
			for (int i = 1; i <= 10; i++)
			{
				GameObject seg = GameObject.Find("segment" + i.ToString("D2"));
				if (seg != null) trunkSegments[i - 1] = seg.transform;
			}
		}

		Vector3 plungerPos = plungerTarget != null
			? plungerTarget.position + Vector3.up * 2.5f
			: transform.TransformPoint(new Vector3(0.42f, 4.5f, 6.06f));
		Vector3 boyPos = boyTarget != null
			? boyTarget.position + Vector3.up * 2.0f
			: transform.TransformPoint(new Vector3(0f, 3.5f, 0f));

		float hitRadiusSq = 3.2f * 3.2f;
		for (int i = 2; i < 10; i++)
		{
			Transform seg = trunkSegments[i];
			if (seg == null) continue;
			Vector3 p = seg.position;
			if ((p - plungerPos).sqrMagnitude <= hitRadiusSq || (p - boyPos).sqrMagnitude <= hitRadiusSq)
			{
				HitByPlayer();
				return;
			}
		}
	}

	private void Update()
	{
		theTime += Time.deltaTime;
		if (theTime > timeToStop && ani != null)
		{
			ani.SetBool("Stop", true);
		}

		// Fallback movement towards endPos if NavMesh is not available
		if (!moveTonextScene && theTime < timeToStop && endPos != null)
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			if (agent == null || !agent.isOnNavMesh)
			{
				float walkSpeed = (agent != null && agent.speed > 0f) ? agent.speed : 8f;
				transform.position = Vector3.MoveTowards(transform.position, endPos.position, walkSpeed * Time.deltaTime);
				Vector3 dir = endPos.position - transform.position;
				dir.y = 0f;
				if (dir.sqrMagnitude > 0.001f)
				{
					transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 180f * Time.deltaTime);
				}
			}
		}

		CheckTrunkProximity();

		if (moveTonextScene)
		{
			RenderSettings.fogEndDistance -= Time.deltaTime * 12f;
			RenderSettings.fogStartDistance -= Time.deltaTime * 15f;
			moveTime += Time.deltaTime;
		}
		if (moveTime > timeToMove)
		{
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
			SceneManager.LoadScene("Main_03");
		}

		Keyboard keyboard = Keyboard.current;
		if (keyboard != null)
		{
			if (keyboard.escapeKey.isPressed)
			{
				Application.Quit();
			}
			if (keyboard.digit1Key.isPressed)
			{
				SceneManager.LoadScene(0);
			}
			if (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
			{
				SceneManager.LoadScene("Main_03");
			}
		}

		if (!canExplode)
		{
			return;
		}
		explodeTime += Time.deltaTime;
		if (explodeTime > timeToExplode)
		{
			if (particle != null)
			{
				Instantiate(particle, transform.position, Quaternion.identity);
			}
			canExplode = false;
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		HitByPlayer();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other != null && (other.gameObject.name.Contains("segment") || LayerMask.LayerToName(other.gameObject.layer) == "Trunk"))
		{
			HitByPlayer();
		}
	}

	private void HitByPlayer()
	{
		if (!moveTonextScene)
		{
			canExplode = true;
			AudioSource audio = GetComponent<AudioSource>();
			if (audio != null)
			{
				audio.Play();
			}
			if (ani != null)
			{
				ani.SetBool("Hit", true);
			}
			moveTonextScene = true;
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			if (agent != null && agent.isOnNavMesh)
			{
				agent.speed = 0f;
			}
		}
	}
}
