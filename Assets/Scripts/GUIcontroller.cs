using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

	public GameObject slowMoPanel;

	void Awake()
	{
		throttle.maxValue = qs.maxThrottle;
		throttle.value = qs.throttle;

		slowMoPanel.SetActive(false);
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


		hight.text = qs.barometr.hight.ToString("0.00");
		throttleText.text = qs.throttle.ToString("0.00");
		vSpeed.text = qs.barometr.verticalSpeed.ToString("0.00");
		hSpeed.text = qs.gps.horizontalSpeed.ToString("0.00");
	}

	string to360Deg(double target)
	{
		if (!qs.stabilizationON) return "-";

		string temp = target >= 0 ? (target - 360 * (int)(target / 360)).ToString("0.0")
									: (360 + target - 360 * (int)(target / 360)).ToString("0.0");
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
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		SceneManager.LoadScene(0);
	}

	static Coroutine slowmoCoroutine = null;

	public void Object()
	{
		Instantiate(obj, qs.gps.coordinates + new Vector3(0f, 3f, 0), Quaternion.identity);

		if(slowmoCoroutine == null) slowmoCoroutine = StartCoroutine(slowMo());
	}

	IEnumerator slowMo()
	{
		slowMoPanel.SetActive(true);
		Time.timeScale = 0.1f;
		Time.fixedDeltaTime /= 10.0f;

		Debug.Log("start slowMo");

		yield return new WaitForSeconds(10.0f*Time.timeScale);

		Time.timeScale = 1.0f;
		Time.fixedDeltaTime *= 10.0f;
		
		Debug.Log("End slowMo");
		slowMoPanel.SetActive(false);

		slowmoCoroutine = null;
		yield return null;
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
