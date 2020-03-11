using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControll : MonoBehaviour
{
	Rigidbody rb;
	Transform quadPos;
	public float speed = 1;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		quadPos = GameObject.Find("Frame").transform;

		Invoke("OnCollisionEnter", 1f);
	}

	void FixedUpdate()
	{
		Vector3 direction = quadPos.position - transform.position;
		rb.AddForce(direction*speed, ForceMode.Impulse);
	}

	void OnCollisionEnter()
	{
		Destroy(gameObject);
	}

}
