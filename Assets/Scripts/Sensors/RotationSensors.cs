using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSensors : MonoBehaviour
{
	//Текущие значения
	public double pitch { get; private set; } //Тангаж
	public double roll { get; private set; } //Крен
	public double yaw { get; private set; } //Рыскание

	void Update()
	{
		Vector3 rot = transform.rotation.eulerAngles;
		pitch = rot.x;
		yaw = rot.y;
		roll = rot.z;
	}
}
