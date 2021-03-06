﻿using UnityEngine;

public class InputControll : MonoBehaviour
{
	quadrocopterScript qs;

	void Awake()
	{
		qs = GetComponent<quadrocopterScript>();
	}

    void Update()
	{
		if(quadrocopterScript.compensation == null)
		{
			if (Input.GetKeyDown(KeyCode.X)) qs.throttleTOzero();
			if (Input.GetKeyDown(KeyCode.Z)) qs.throttleTOmax();
			if (Input.GetKeyDown(KeyCode.Space)) qs.zeroPitchAndRoll();
			if (Input.GetKeyDown(KeyCode.O)) qs.switchStabilization();
			if (Input.GetKeyDown(KeyCode.T)) qs.hovering();
			if (Input.GetKeyDown(KeyCode.L)) StopAllCoroutines();
		}
	}

	void FixedUpdate()
	{
		if (quadrocopterScript.compensation == null) inputController();
	}

	void inputController()
	{
		qs.throttle += Input.GetAxis("throttle") * qs.throttleStep;

		qs.targetPitch += Input.GetAxis("pitch") * qs.targetStep;
		qs.targetRoll += Input.GetAxis("roll") * qs.targetStep;
		qs.targetYaw += Input.GetAxis("yaw") * qs.targetStep;

		//_____ УПРАВЛЕНИЕ АНДРОИД
		//targetPitch += Input.acceleration.x * targetStep;
		//targetYaw += Input.acceleration.y * targetStep;
		//targetRoll += Input.acceleration.z * targetStep;
	}
}
