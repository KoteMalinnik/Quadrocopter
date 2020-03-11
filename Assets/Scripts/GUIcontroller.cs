using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIcontroller : MonoBehaviour
{
	public Slider throttel;
	public Toggle stabilization;
	public Toggle wind;
	public Toggle obj;

	public quadrocopterScript qs;

	void Awake()
	{
		throttel.maxValue = (float)qs.maxThrottle;
		throttel.value = (float)qs.throttle;
	}

	public void changeThrottle()
	{
		qs.throttle = throttel.value;
	}

	public void switchStabilization()
	{
		qs.stabilizationON = stabilization.isOn;
	}
}
