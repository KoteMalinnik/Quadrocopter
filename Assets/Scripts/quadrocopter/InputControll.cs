using UnityEngine;

public class InputControll : MonoBehaviour
{
	quadrocopterScript qs;

	void Awake()
	{
		qs = GetComponent<quadrocopterScript>();
	}

    void Update()
	{
		if (Input.GetKeyDown(KeyCode.X)) qs.throttleTOzero();
		if (Input.GetKeyDown(KeyCode.Z)) qs.throttleTOmax();
		if (Input.GetKeyDown(KeyCode.Space)) qs.zeroPitchAndRoll();
		if (Input.GetKeyDown(KeyCode.O)) qs.switchStabilization();
		if (Input.GetKeyDown(KeyCode.T)) qs.hovering();
		if (Input.GetKeyDown(KeyCode.L)) StopAllCoroutines();
	}

	void FixedUpdate()
	{
		inputController();
	}

	void inputController()
	{
		qs.throttle += Input.GetAxis("throttle") * qs.throttleStep;
		qs.throttle = qs.throttle > qs.maxThrottle ? qs.maxThrottle : qs.throttle;
		qs.throttle = qs.throttle < 0 ? 0 : qs.throttle;

		qs.targetPitch += Input.GetAxis("pitch") * qs.targetStep;
		qs.targetYaw += Input.GetAxis("yaw") * qs.targetStep;
		qs.targetRoll += Input.GetAxis("roll") * qs.targetStep;

		//_____ УПРАВЛЕНИЕ С АНДРОИД

		//Debug.Log($"x: {Input.acceleration.x}\n" +
		//          $"y: {Input.acceleration.y}\n" +
		//          $"z: {Input.acceleration.z}");

		//targetPitch += Input.acceleration.x * targetStep;
		//targetYaw += Input.acceleration.y * targetStep;
		//targetRoll += Input.acceleration.z * targetStep;

	}
}
