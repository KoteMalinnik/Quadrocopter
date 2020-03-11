using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIcontroller : MonoBehaviour
{
	public Slider throttel;
	public Toggle stabilization;
	public Toggle wind;
	public Toggle obj;

	public quadrocopterScript qs;

	public Text currentPitch, targetPitch;
	public Text currentRoll, targetRoll;
	public Text currentYaw, targetYaw;

	public Text hight;

	void Awake()
	{
		throttel.maxValue = (float)qs.maxThrottle;
		throttel.value = (float)qs.throttle;
	}

	void Update()
	{
		currentPitch.text = qs.rotation.pitch.ToString(".00");
		targetPitch.text = to360Deg(qs.targetPitch);

		currentRoll.text = qs.rotation.roll.ToString(".00");
		targetRoll.text = to360Deg(qs.targetRoll);

		currentYaw.text = qs.rotation.yaw.ToString(".00");
		targetYaw.text = to360Deg(qs.targetYaw);

		hight.text = qs.barometr.hight.ToString(".00");

		throttel.value = (float)qs.throttle;
	}

	string to360Deg(double target)
	{
		string temp = target >= 0 ? (target - 360 * (int)(target / 360)).ToString(".00") : (360 + target - 360 * (int)(target / 360)).ToString(".00");
		return temp;
	}

	public void changeThrottle()
	{
		qs.throttle = throttel.value;
	}

	public void switchStabilization()
	{
		qs.stabilizationON = stabilization.isOn;
	}

	public void Restart()
	{
		SceneManager.LoadScene(0);
	}
}
