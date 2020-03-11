using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIcontroller : MonoBehaviour
{
	public GameObject obj;

	[Header("GUI")]

	public Slider throttle;
	public Toggle stabilization;
	public Toggle wind;

	public quadrocopterScript qs;

	public Text currentPitch, targetPitch;
	public Text currentRoll, targetRoll;
	public Text currentYaw, targetYaw;

	public Text hight;
	public Text throttleText;
	public Text hSpeed;
	public Text vSpeed;

	public Dropdown stabilizationType;

	void Awake()
	{
		throttle.maxValue = qs.maxThrottle;
		throttle.value = qs.throttle;
	}

	void Update()
	{
		throttle.value = qs.throttle;

		currentPitch.text = qs.rotation.pitch.ToString("0.0");
		targetPitch.text = to360Deg(qs.targetPitch);

		currentRoll.text = qs.rotation.roll.ToString("0.0");
		targetRoll.text = to360Deg(qs.targetRoll);

		currentYaw.text = qs.rotation.yaw.ToString("0.0");
		targetYaw.text = to360Deg(qs.targetYaw);


		hight.text = qs.barometr.hight.ToString("0.0");
		throttleText.text = qs.throttle.ToString("0.0");
		vSpeed.text = qs.barometr.verticalSpeed.ToString("0.0");
		hSpeed.text = qs.gps.horizontalSpeed.ToString("0.0");
	}

	string to360Deg(double target)
	{
		if (!qs.stabilizationON) return "-";

		string temp = target >= 0 ? (target - 360 * (int)(target / 360)).ToString("0.0") : (360 + target - 360 * (int)(target / 360)).ToString("0.0");
		return temp;
	}

	public void changeThrottle()
	{
		qs.throttle = throttle.value;
	}

	public void switchStabilization()
	{
		qs.switchStabilization();
	}

	public void Restart()
	{
		SceneManager.LoadScene(0);
	}

	public void Object()
	{
		Instantiate(obj, qs.gps.coordinates + new Vector3(0f, 3f, 0), Quaternion.identity);
	}

	public void Hovering()
	{
		qs.hovering();
	}

	public void changeStabilizationType()
	{
		if(stabilizationType.value<1) qs.changeStabilizationTYPE(true);
		else qs.changeStabilizationTYPE(false);
	}
}
