using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSensors : MonoBehaviour
{
	//Текущие значения
	public float pitch { get; private set; } //Тангаж
	public float roll { get; private set; } //Крен
	public float yaw { get; private set; } //Рыскание

	void Update()
	{
		Vector3 rot = transform.rotation.eulerAngles;
		pitch = rot.x;
		yaw = rot.y;
		roll = rot.z;
	}
}
