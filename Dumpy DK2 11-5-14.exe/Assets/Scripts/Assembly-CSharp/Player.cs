using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	public GameObject trunk;
	public GUIStyle guiStyle;
	public AudioClip[] roars;
	public float roarTimer = 45f;
	public float speed = 6f;
	private float upSpeed = 10f;
	private float newRoarTimer;
	private float timeToRoar;
	private bool triggered;
	public float timeToMove = 5f;
	private float moveTime;
	private float endTime;
	private float timeToEnd = 15f;
	private bool endTimerOn;
	private bool goBlack;
	public float blackScreenTime;
	private float timeInBlaskScreen;
	private Color color1;
	private Color color2;
	private Color color3;
	private float startVolume;
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource != null)
		{
			startVolume = audioSource.volume;
		}
		color1 = new Color(1f, 0.25f, 0.5f, 1f);
		color2 = new Color(1f, 0.75f, 0.3f, 1f);
		color3 = new Color(1f, 0f, 0.8f, 1f);
	}

	private void Update()
	{
		timeToRoar += Time.deltaTime;
		if (timeToRoar > newRoarTimer)
		{
			newRoarTimer = Random.Range(roarTimer * 0.75f, roarTimer * 1.25f);
			if (audioSource != null && roars != null && roars.Length > 0)
			{
				audioSource.PlayOneShot(roars[Random.Range(0, roars.Length)]);
			}
			timeToRoar = 0f;
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
				SceneManager.LoadScene(1);
			}
		}
		transform.position += Time.deltaTime * transform.forward * speed;
		if (triggered)
		{
			moveTime += Time.deltaTime;
			if (moveTime > timeToMove)
			{
				transform.position = new Vector3(0f, -500f, 0f);
				speed = 0f;
				upSpeed = 0f;
				triggered = false;
				goBlack = true;
				if (trunk != null)
				{
					trunk.transform.position = transform.position;
				}
				DumpyTrunk dumpyTrunk = GetComponent<DumpyTrunk>();
				if (dumpyTrunk == null) dumpyTrunk = Object.FindObjectOfType<DumpyTrunk>();
				if (dumpyTrunk != null)
				{
					dumpyTrunk.TeleportToHead();
				}
				ufoMother mother = Object.FindObjectOfType<ufoMother>();
				if (mother != null)
				{
					mother.enabled = false;
				}
				Physics.SyncTransforms();
			}
			else
			{
				transform.position += Time.deltaTime * new Vector3(0f, 1f, 1f) * upSpeed;
			}
		}
		if (goBlack)
		{
			timeInBlaskScreen += Time.deltaTime;
			if (audioSource != null && blackScreenTime > 0f)
			{
				audioSource.volume = Mathf.Lerp(startVolume, 0f, timeInBlaskScreen / blackScreenTime);
			}
			if (timeInBlaskScreen > blackScreenTime)
			{
				goBlack = false;
				endTimerOn = true;
				RenderSettings.fogStartDistance = 50f;
				RenderSettings.fogEndDistance = 200f;
			}
		}
		if (endTimerOn)
		{
			endTime += Time.deltaTime;
			RenderSettings.fogEndDistance -= Time.deltaTime * 5f;
			RenderSettings.fogStartDistance -= Time.deltaTime * 5f;
			if (endTime > timeToEnd)
			{
				endTimerOn = false;
				SceneManager.LoadScene(2);
			}
		}
	}

	private void TriggerEnter()
	{
		triggered = true;
		speed = 0f;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col != null && col.gameObject.name == "EndTrigger")
		{
			TriggerEnter();
		}
	}
}
