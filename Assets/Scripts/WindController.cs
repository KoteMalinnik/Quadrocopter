using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
	[SerializeField]
	float windForce = 1f;
	[SerializeField]
	Rigidbody target;

	static bool isOn = false;


	static Vector3 windDirection = Vector3.zero;
	public static void toggleWind(bool newIsOn)
	{
		isOn = newIsOn;

		if (!isOn) return; //Если ветер выключили, то код ниже не выполнится 

		windDirection = Vector3.one * 5;
		windDirection.x *= Random.Range(0.1f, 0.7f);
		windDirection.y *= Random.Range(0.1f, 0.7f);
		windDirection.z *= Random.Range(0.1f, 0.7f);
	}

    void FixedUpdate()
    {
		if(isOn)
		{
			target.AddRelativeForce(windDirection*windForce, ForceMode.Force);	
		}
    }
}
