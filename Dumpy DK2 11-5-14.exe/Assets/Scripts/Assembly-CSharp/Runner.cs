using UnityEngine;

internal class Runner : Actor
{
	public bool startTriggered;
	public float animationSpeed;
	public float speed;
	public float lookSpeed;
	public float moveDist;
	public float minDist;
	public float hitVelocity;
	public float tipPercent;
	public float timeToExplode;
	public GameObject brokenObj;
	public GameObject breakSound;
	public GameObject runParticle;
	public GameObject hitParticle;
	public GameObject yell;
}
