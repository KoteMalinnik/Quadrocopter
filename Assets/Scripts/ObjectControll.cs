using UnityEngine;
using System.Collections;

public class ObjectControll : MonoBehaviour
{
	Rigidbody rb;
	Transform quadPos;
	public float speed = 10;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		quadPos = GameObject.Find("Frame").transform;
	}

	void Update()
	{
		var direction = quadPos.position - transform.position;
		rb.AddForce(direction * speed, ForceMode.Force);
	}

	void OnCollisionEnter()
	{
		Debug.Log("Столкновение");
	}
}
