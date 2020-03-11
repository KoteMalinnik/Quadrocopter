using UnityEngine;

public class motorScript : MonoBehaviour {

	public double power = 0.0f;

	Rigidbody rb = null;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		rb.AddRelativeForce (0, (float)power, 0);
	}
}
