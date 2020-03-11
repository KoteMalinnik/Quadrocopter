using UnityEngine;
using System;
using System.Collections;

public class quadrocopterScript : MonoBehaviour {

	public GUIcontroller gui { get; private set; }
	public GPS gps { get; private set; }
	public Barometr barometr { get; private set; }
	public RotationSensors rotation { get; private set; }

	public bool stabilizationON = true;
	bool horizontalStabilization = true;

	[Space]
	public float throttle; //Тяга
	public float maxThrottle = 25;
	public float throttleStep = 0.01f;

	public float k_classicInput = 1f; //коэффициент изменения скорости двигателей

	[Space]
	public float targetStep = 0.01f;
	public float targetPitch { get; set; }
	public float targetRoll { get; set; }
	public float targetYaw { get; set; }

	PID pitchPID = new PID();
	[Header("pitch/тангаж")]
	public float p_P = 80;
	public float p_I = 0;
	public float p_D = 20;

	PID rollPID = new PID();
	[Header("roll/крен")]
	public float r_P = 80;
	public float r_I = 0;
	public float r_D = 20;

	PID yawPID = new PID();
	[Header("yaw/рысканье")]
	public float y_P = 50;
	public float y_I = 0;
	public float y_D = 80;

	motorScript motor1;
	motorScript motor2;
	motorScript motor3;
	motorScript motor4;

	Rigidbody rb;

	void Awake()
	{
		GameObject frame = GameObject.Find("Frame");
		rb = frame.GetComponent<Rigidbody>();
		barometr = frame.GetComponent<Barometr>();
		gps = frame.GetComponent<GPS>();
		rotation = frame.GetComponent<RotationSensors>();

		gui = GameObject.Find("Canvas").GetComponent<GUIcontroller>();

		motor1 = GameObject.Find("Motor1").GetComponent<motorScript>();
		motor2 = GameObject.Find("Motor2").GetComponent<motorScript>();
		motor3 = GameObject.Find("Motor3").GetComponent<motorScript>();
		motor4 = GameObject.Find("Motor4").GetComponent<motorScript>();
	}

	//void Update()
	//{
	//	gps.hSpeedVector
	//	var hDir = new Vector3(gps.direction.x, 0, gps.direction.z);
	//	hDir = hDir.normalized;
	//	var forwardV = new Vector3(gps.coordinates.x, 0, gps.coordinates.z);
	//	Vector3.forward

	//	var delta = Vector3.Angle(Vector3.forward, hDir);
		
	//	Debug.Log(forwardV);
	//}

	//Вычисления физики в FixedUpdate, а не в Update
	void FixedUpdate()
	{
		//1 и 2 мотор впереди
		//3 и 4 моторы сзади
		motor1.power = throttle;
		motor2.power = throttle;
		motor3.power = throttle;
		motor4.power = throttle;

		if (stabilizationON) stabilize();
		else classicControll();
	}

	void motors(float mp1, float mp2, float mp3, float mp4)
	{
		motor1.power += mp1;
		motor2.power += mp2;
		motor3.power += mp3;
		motor4.power += mp4;
	}

	void classicControll()
	{
		float pitch = k_classicInput * Input.GetAxis("pitch");
		float roll = k_classicInput * Input.GetAxis("roll");
		float yaw = k_classicInput * Input.GetAxis("yaw");

		motors(-pitch, -pitch, pitch, pitch); 
		motors(-roll, roll, roll, -roll);
		motors(yaw, -yaw, yaw, -yaw);
	}

	//функция стабилизации квадрокоптера
	//с помощью PID регуляторов мы настраиваем
	//мощность наших моторов так, чтобы углы приняли нужные нам значения
	void stabilize ()
	{
		//нам необходимо посчитать разность между требуемым углом и текущим
		//эта разность должна лежать в промежутке [-180, 180] чтобы обеспечить
		//правильную работу PID регуляторов, так как нет смысла поворачивать на 350
		//градусов, когда можно повернуть на -10
		float dPitch = targetPitch - rotation.pitch;
		float dRoll = targetRoll - rotation.roll;
		float dYaw = targetYaw - rotation.yaw;

		dPitch -= (float)(Math.Ceiling(Math.Floor(dPitch / 180.0) / 2.0) * 360.0);
		dRoll -= (float)(Math.Ceiling(Math.Floor(dRoll / 180.0) / 2.0) * 360.0);
		dYaw -= (float)(Math.Ceiling(Math.Floor(dYaw / 180.0) / 2.0) * 360.0);

		//ограничитель на мощность подаваемую на моторы,
		//чтобы в сумме мощность всех моторов оставалась
		//одинаковой при регулировке
		float powerLimit = throttle > 20 ? 20 : throttle;

		//управление тангажем:
		//на передние двигатели подаем возмущение от регулятора
		//на задние противоположное возмущение
		float pitchForce = pitchPID.Calculate(p_P, p_I, p_D, 0f, dPitch / 180.0f, stabilizationON);
		pitchForce = saturation(pitchForce, powerLimit);
		motors(-pitchForce, -pitchForce, pitchForce, pitchForce);

		//управление креном:
		//действуем по аналогии с тангажем, только регулируем боковые двигатели
		float rollForce = rollPID.Calculate(r_P, r_I, r_D, 0, dRoll / 180.0, stabilizationON);
		rollForce = saturation(rollForce, powerLimit);
		motors(-rollForce, rollForce, rollForce, -rollForce);

		//управление рысканием:
		float yawForce = yawPID.Calculate(y_P, y_I, y_D, 0, dYaw / 180.0, stabilizationON);
		yawForce = saturation(yawForce, powerLimit);
		motors(yawForce, -yawForce, yawForce, -yawForce);
	}

	//Насыщение
	float saturation(float current, float limit)
	{
		current = current > limit ? limit : current;
		current = current < -limit ? -limit : current;

		return deadZone(current);
	}

	//зона нечувствительности
	float deadZone(float val)
	{
		val = Math.Abs(val) < 0.001 ? 0 : val;
		return val;
	}

	public void throttleTOzero()
	{
		Debug.Log("Нулевая тяга");
		throttle = 0;
	}

	public void throttleTOmax()
	{
		Debug.Log("Тяга на максимум");
		throttle = maxThrottle;
	}

	public void zeroPitchAndRoll()
	{
		Debug.Log("Обнуление крена и тангажа");
		targetRoll = 0;
		targetPitch = 0;
	}


	public void changeStabilizationTYPE(bool isHorizontal)
	{
		if (isHorizontal) horizontalStabilization = true;
		else horizontalStabilization = false;
	}

	public void switchStabilization()
	{
		if(horizontalStabilization)
		{
			zeroPitchAndRoll();
		}
		else
		{
			targetPitch = rotation.pitch;
			targetRoll = rotation.roll;
			targetYaw = rotation.yaw;
		}

		stabilizationON = !stabilizationON;
		Debug.Log($"Стабилизация: {stabilizationON}");
	}

	public void hovering()
	{
		Debug.Log("Зависание");
		//StartCoroutine(hover());
	}

	IEnumerator hover()
	{
		float dt = Time.fixedDeltaTime;

		zeroPitchAndRoll();

		var hDir = new Vector3(gps.direction.x, 0, gps.direction.z);
		var yawVector = new Vector2(rotation.yaw, 0);

		var delta = Vector3.Angle(Vector3.forward, hDir);
		Debug.Log(delta);
		//targetYaw += delta;

		//while( gps.horizontalSpeed > 0.1)
		//{
			
		//	yield return new WaitForSeconds(Time.fixedDeltaTime);
		//}

		Debug.Log("Завис");

		yield return null;
	}
}