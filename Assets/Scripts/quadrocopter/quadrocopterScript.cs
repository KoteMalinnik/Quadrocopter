using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class quadrocopterScript : MonoBehaviour {

	public GUIcontroller gui { get; private set; }
	public GPS gps { get; private set; }
	public Barometr barometr { get; private set; }
	public RotationSensors rotation { get; private set; }

	public bool stabilizationON = true;
	bool horizontalStabilization = true;

	//коэффициенты подобраны идеально
	//[Header("ПИД вертикальной скорости")]
	float vs_P { get; } = 0.5f;
	float vs_I { get; } = 0f;
	float vs_D { get; } = 0.2f;
	float maxVertDelta { get; } = 10f;

	//Подобрать точнее
	[Header("ПИД горизонтальной скорости")]
	public float hs_P = 10f;
	public float hs_I = 0f;
	public float hs_D = 0f;
	public float maxHorAngle = 70f;

	[Header("Тяга")]	public float throttle = 22.41293f; //Тяга нулевой вертикальной скорости
	public float maxThrottle { get; } = 50;
	public float throttleStep { get; } = 0.5f;

	public float targetStep { get; } = 5f;
	public float targetPitch { get; set; } = 0;
	public float targetRoll { get; set; } = 0;
	public float targetYaw { get; set; } = 0;

	//Подобрать точнее
	PID pitchPID = new PID();
	[Header("pitch/тангаж")]
	public float p_P = 80;
	public float p_I = 0;
	public float p_D = 20;

	//Подобрать точнее
	PID rollPID = new PID();
	[Header("roll/крен")]
	public float r_P = 80;
	public float r_I = 0;
	public float r_D = 20;

	//Подобрать точнее
	PID yawPID = new PID();
	[Header("yaw/рысканье")]
	public float y_P = 80;
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

	//Вычисления физики в FixedUpdate, а не в Update
	void FixedUpdate()
	{
		//1 и 2 мотор впереди
		//3 и 4 моторы сзади
		motor1.power = throttle;
		motor2.power = throttle;
		motor3.power = throttle;
		motor4.power = throttle;

		float maxAngle = 60;
		//Ограничение задаваемого тангажа и крена
		targetPitch = saturation(targetPitch, maxAngle);
		targetRoll = saturation(targetRoll, maxAngle);


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
		float pitch = Input.GetAxis("pitch");
		float roll = Input.GetAxis("roll");
		float yaw = Input.GetAxis("yaw");

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
		Debug.Log("Тяга на ноль");
		throttle = 0;
	}

	public void throttleTOmax()
	{
		Debug.Log("Тяга на максимум");
		throttle = maxThrottle;
	}

	public void zeroPitchAndRoll()
	{
		Debug.Log("Выравнивание по нулевым крену и тангажу");
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

		StopAllCoroutines();
		stabilizationON = !stabilizationON;
		gui.stabilization.isOn = stabilizationON;
		Debug.Log($"Стабилизация: {stabilizationON}");
	}

	public static Coroutine compensation { get; private set; } = null;
	public void hovering()
	{
		Debug.Log("Зависание. Нажмите <L> для отмены");
		compensation = StartCoroutine(SpeedCompensation());
	}

	IEnumerator SpeedCompensation()
	{
		Debug.Log("Компенсация горизонтальной и вертикальной скорости");
		var horSpeedPid = new PID();
		var verSpeedPID = new PID();

		zeroPitchAndRoll();
		targetYaw = 0;

		while (!Input.GetKeyDown(KeyCode.L) && GUIcontroller.isHovering)
		{
			//Горизонтальная компенсация
			var newPitch = horSpeedPid.Calculate(hs_P, hs_I, hs_D, gps.horSpeedZ, 0f);
			var newRoll = horSpeedPid.Calculate(hs_P, hs_I, hs_D, gps.horSpeedX, 0f);

			newPitch = saturation(newPitch, maxHorAngle);
			newRoll = saturation(newRoll, maxHorAngle);

			targetPitch = newPitch;
			targetRoll = -newRoll;

			//Вертикальная компенсация
			var deltaThrottel = verSpeedPID.Calculate(vs_P, vs_I, vs_D, barometr.verticalSpeed, 0);
			deltaThrottel = saturation(deltaThrottel, maxVertDelta);

			throttle += deltaThrottel;
			throttle = saturation(throttle, maxThrottle);
			//Устанавливаем в 1, чтобы отсутствие тяги не мешало коптеру стабилизироваться
			if (throttle < 1) throttle = 1;

			yield return new WaitForFixedUpdate();
		}

		Debug.Log("Это должно появиться в консоли только после нажатия L");
		compensation = null;
		yield return null;
	}
}