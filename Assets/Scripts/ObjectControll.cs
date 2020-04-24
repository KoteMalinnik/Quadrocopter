using UnityEngine;
using System.Collections;

public class ObjectControll : MonoBehaviour
{
	StabilizationTimer timer;

	Rigidbody rb;
	Transform targetTransform;

	[SerializeField]
	float speed = 100;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		targetTransform = GameObject.Find("Frame").transform;
		timer = GameObject.Find("Frame").GetComponent<StabilizationTimer>();
	}

	void Update()
	{
		var direction = targetTransform.position - transform.position;
		rb.AddForce(direction * speed, ForceMode.Force);
	}

	void OnCollisionEnter()
	{
		Debug.Log("Столкновение");
		timer.startTimer();
		Destroy(gameObject);
	}
}
