using UnityEngine;

public abstract class Actor : MonoBehaviour
{
	public string triggerName;
	protected Animator ani;
	protected Rigidbody rig;

	public abstract void Trigger();

	public void Start()
	{
		ani = GetComponent<Animator>();
		rig = GetComponent<Rigidbody>();
	}
}
