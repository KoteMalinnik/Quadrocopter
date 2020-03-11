using UnityEngine;
using System;
using System.Collections;

public class quadrocopterScript : MonoBehaviour {

	public GUIcontroller gui;

	public bool stabilizationON = true;

	//фактические параметры
	public double pitch { get; private set; } //Тангаж
	public double roll { get; private set; } //Крен
	public double yaw { get; private set; } //Рыскание

	public double hight { get; private set; } //высота

	[Space]
	public double throttle; //Тяга
	public double maxThrottle = 25;
	public double throttleStep = 0.01;

	[Space]
	public double targetStep = 0.01;
	public double targetPitch { get; set; }
	public double targetRoll { get; set; }
	public double targetYaw { get; set; }

	PID pitchPID = new PID();
	[Header("pitch/тангаж")]
	public double p_P = 80;
	public double p_I = 0;
	public double p_D = 20;

	PID rollPID = new PID();
	[Header("roll/крен")]
	public double r_P = 80;
	public double r_I = 0;
	public double r_D = 20;

	PID yawPID = new PID();
	[Header("yaw/рысканье")]
	public double y_P = 50;
	public double y_I = 0;
	public double y_D = 80;

	Transform frame;
	motorScript motor1;
	motorScript motor2;
	motorScript motor3;
	motorScript motor4;

	Rigidbody rb;

	void Awake()
	{
		rb = GameObject.Find("Frame").GetComponent<Rigidbody>();
		frame = GameObject.Find("Frame").GetComponent<Transform>();

		motor1 = GameObject.Find("Motor1").GetComponent<motorScript>();
		motor2 = GameObject.Find("Motor2").GetComponent<motorScript>();
		motor3 = GameObject.Find("Motor3").GetComponent<motorScript>();
		motor4 = GameObject.Find("Motor4").GetComponent<motorScript>();
	}

	void Update()
	{
		hight = frame.position.y;

		readRotation(); //чтение данных с акселерометра квадрокоптера
	}

	//Вычисления физики в FixedUpdate, а не в Update
	void FixedUpdate()
	{
		if (stabilizationON) stabilize();
	}

	void readRotation()
	{
		//фактическая ориентация нашего квадрокоптера,
		//в реальном квадрокоптере эти данные необходимо получать
		//из акселерометра-гироскопа-магнетометра, так же как делает это ваш смартфон
		Vector3 rot = frame.rotation.eulerAngles;
		pitch = rot.x;
		yaw = rot.y;
		roll = rot.z;
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

		double dPitch = targetPitch - pitch;
		double dRoll = targetRoll - roll;
		double dYaw = targetYaw - yaw;

		dPitch -= Math.Ceiling(Math.Floor(dPitch / 180.0) / 2.0) * 360.0;
		dRoll -= Math.Ceiling(Math.Floor(dRoll / 180.0) / 2.0) * 360.0;
		dYaw -= Math.Ceiling(Math.Floor(dYaw / 180.0) / 2.0) * 360.0;

		//1 и 2 мотор впереди
		//3 и 4 моторы сзади
		double motor1power = throttle;
		double motor2power = throttle;
		double motor3power = throttle;
		double motor4power = throttle;

		//ограничитель на мощность подаваемую на моторы,
		//чтобы в сумме мощность всех моторов оставалась
		//одинаковой при регулировке
		double powerLimit = throttle > 20 ? 20 : throttle;

		//управление тангажем:
		//на передние двигатели подаем возмущение от регулятора
		//на задние противоположное возмущение
		double pitchForce = pitchPID.Calculate(p_P, p_I, p_D, 0, dPitch / 180.0, stabilizationON);
		pitchForce = saturation(pitchForce, powerLimit);
		motor1power += -pitchForce;
		motor2power += -pitchForce;
		motor3power += pitchForce;
		motor4power += pitchForce;

		//управление креном:
		//действуем по аналогии с тангажем, только регулируем боковые двигатели
		double rollForce = rollPID.Calculate(r_P, r_I, r_D, 0, dRoll / 180.0, stabilizationON);
		rollForce = saturation(rollForce, powerLimit);
		motor1power += -rollForce;
		motor2power += rollForce;
		motor3power += rollForce;
		motor4power += -rollForce;

		//управление рысканием:
		double yawForce = yawPID.Calculate(y_P, y_I, y_D, 0, dYaw / 180.0, stabilizationON);
		yawForce = saturation(yawForce, powerLimit);
		motor1power += yawForce;
		motor2power += -yawForce;
		motor3power += yawForce;
		motor4power += -yawForce;

		motor1.power = motor1power;
		motor2.power = motor2power;
		motor3.power = motor3power;
		motor4.power = motor4power;
	}

	//Насыщение
	double saturation(double current, double limit)
	{
		current = current > limit ? limit : current;
		current = current < -limit ? -limit : current;

		return current;
	}

	//IEnumerator hovering()
	//{
		//Debug.Log("Зависание");
	//	yield return new WaitWhile(() => roll > 0.01 && pitch > 0.01);
	//	Debug.Log("Выровнялся по крену и тангажу");

	//	while( Mathf.Abs( (float)accelerometr) > accelerometrThreshold)
	//	{
	//		Debug.Log(accelerometr);
	//		targetPitch += pitchStep;
	//		throttle -= step;
	//		yield return new WaitForSeconds(Time.fixedDeltaTime);
	//	}

	//	targetPitch = 0;

	//	Debug.Log("Завис");
	//	Debug.Break();
	//}


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

	public void switchStabilization()
	{
		stabilizationON = !stabilizationON;
		gui.stabilization.isOn = stabilizationON;

		Debug.Log($"Переключение стабилизации: {stabilizationON}");
	}

	public void hovering()
	{
		zeroPitchAndRoll();

		//StartCoroutine(hovering());
	}
}