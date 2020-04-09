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

	public static void toggleWind(bool newIsOn)
	{
		isOn = newIsOn;
	}

    void FixedUpdate()
    {
		if(isOn)
		{
			target.AddRelativeForce(Vector3.forward*windForce, ForceMode.Force);	
		}
    }
}
