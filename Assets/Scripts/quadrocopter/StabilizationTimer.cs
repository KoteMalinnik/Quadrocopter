using System.Collections;
using UnityEngine;

public class StabilizationTimer : MonoBehaviour
{
	Rigidbody frame;
	void Awake()
	{
		frame = GameObject.Find("Frame").GetComponent<Rigidbody>();
	}

	float time = 0;
	public void startTimer()
	{
		time = 0;
		StartCoroutine(timer());
	}

	IEnumerator timer()
	{
		//Таймер будет считать, скалярная сумма вектора скорости коптера не будет меньше либо равно 0.01f
		while(frame.velocity.magnitude > 0.01f)
		{
			yield return new WaitForFixedUpdate();
			time += Time.fixedDeltaTime;
		}

		Debug.Log("Время стабилизации: " + time + "c");
	}
}
