using UnityEngine;

public class ObjectControll : MonoBehaviour
{
	Rigidbody rb;
	Transform quadPos;
	public float speed = 1;
	public static int counter = 0;

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
		counter++;

		if(counter==1)
		{
			Debug.Log("Столкновение");
			Destroy(gameObject);
		}
		
	}

	void OnDestroy()
	{
		counter = 0;
	}

}
