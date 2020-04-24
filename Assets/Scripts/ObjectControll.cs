using UnityEngine;
using System.Collections;

public class ObjectControll : MonoBehaviour
{
	StabilizationTimer timer;
	Rigidbody rb;
	Transform quadPos;
	public float speed = 10;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		quadPos = GameObject.Find("Frame").transform;
		timer = GameObject.Find("Frame").GetComponent<StabilizationTimer>();
	}

	void Update()
	{
		var direction = quadPos.position - transform.position;
		rb.AddForce(direction * speed, ForceMode.Force);
	}

	void OnCollisionEnter()
	{
		timer.startTimer();
		Debug.Log("Столкновение");
		Destroy(gameObject);
	}
}
