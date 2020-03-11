using UnityEngine;
using System;
using UnityEditor;
using System.Collections;

public class quadrocopterScript : MonoBehaviour {

	public GUIcontroller gui;

	public bool stabilizationON = true;

	public double accelerometr = 0;
	public double accelerometrThreshold = 0.1;
	public double pitchStep = 0.01;
	public double step = 0.01;

	//фактические параметры
	double pitch; //Тангаж
	double roll; //Крен
	double yaw; //Рыскание

	//требуемые параметры	public double throttle; //Тяга

	[Space]

	public double maxThrottle = 25;
	public double throttleStep = 0.01;

	public double targetStep = 0.01;
	public double targetPitch;
	public double targetRoll;
	public double targetYaw;


	[Header("pitch/тангаж")]
	public double p_P = 80;
	public double p_I = 0;
	public double p_D = 20;

	[Header("roll/крен")]
	public double r_P = 80;
	public double r_I = 0;
	public double r_D = 20;

	[Header("yaw/рысканье")]
	public double y_P = 50;
	public double y_I = 0;
	public double y_D = 80;


	Quaternion prevRotation = new Quaternion (0, 1, 0, 0);

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

	void readSensors()
	{
		accelerometr = rb.velocity.y;
	}

	//функция стабилизации квадрокоптера
	//с помощью PID регуляторов мы настраиваем
	//мощность наших моторов так, чтобы углы приняли нужные нам значения
	void stabilize ()
	{
		if(stabilizationON)
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
			double pitchForce = -PID.Calculate(p_P, p_I, p_D, 0, dPitch / 180.0);
			pitchForce = pitchForce > powerLimit ? powerLimit : pitchForce;
			pitchForce = pitchForce < -powerLimit ? -powerLimit : pitchForce;
			motor1power += pitchForce;
			motor2power += pitchForce;
			motor3power += -pitchForce;
			motor4power += -pitchForce;

			//управление креном:
			//действуем по аналогии с тангажем, только регулируем боковые двигатели
			double rollForce = -PID.Calculate(r_P, r_I, r_D, 0, dRoll / 180.0);
			rollForce = rollForce > powerLimit ? powerLimit : rollForce;
			rollForce = rollForce < -powerLimit ? -powerLimit : rollForce;
			motor1power += rollForce;
			motor2power += -rollForce;
			motor3power += -rollForce;
			motor4power += rollForce;

			//управление рысканием:
			double yawForce = PID.Calculate(y_P, y_I, y_D, 0, dYaw / 180.0);
			yawForce = yawForce > powerLimit ? powerLimit : yawForce;
			yawForce = yawForce < -powerLimit ? -powerLimit : yawForce;
			motor1power += yawForce;
			motor2power += -yawForce;
			motor3power += yawForce;
			motor4power += -yawForce;

			motor1.power = motor1power;
			motor2.power = motor2power;
			motor3.power = motor3power;
			motor4.power = motor4power;
		}
	}



	void inputController()
	{
		#if UNITY_EDITOR
		throttle += Input.GetAxis("throttle")*throttleStep;
		throttle = throttle > maxThrottle ? maxThrottle : throttle;
		throttle = throttle < 0 ? 0 : throttle;

		targetPitch += Input.GetAxis("pitch") * targetStep;
		targetYaw += Input.GetAxis("yaw") * targetStep;
		targetRoll += Input.GetAxis("roll") * targetStep;



		#else
		//_____ УПРАВЛЕНИЕ С АНДРОИД

		//изменять тягу слайдером
		//throttle += ;


		Debug.Log($"x: {Input.acceleration.x}\n" +
		          $"y: {Input.acceleration.y}\n" +
		          $"z: {Input.acceleration.z}");

		//targetPitch += Input.acceleration.x * targetStep;
		//targetYaw += Input.acceleration.y * targetStep;
		//targetRoll += Input.acceleration.z * targetStep;
		#endif
	}

	//Вычисления физики в FixedUpdate, а не в Update
	void FixedUpdate()
	{
		inputController();
		readRotation();
		readSensors();

		stabilize();
	}


	void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			Debug.Log("Нулевая тяга");
			throttle = 0;
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Debug.Log("Тяга на максимум");
			throttle = maxThrottle;
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("Обнуление крена и тангажа");
			targetRoll = 0;
			targetPitch = 0;
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			stabilizationON = !stabilizationON;
			gui.stabilization.isOn = stabilizationON;
			Debug.Log($"Переключение стабилизации: {stabilizationON}");
		}


		if(Input.GetKeyDown(KeyCode.T))
		{
			targetRoll = 0;
			targetPitch = 0;
			Debug.Log("Зависание");

			StartCoroutine(hovering());
		}
	}

	IEnumerator hovering()
	{
		yield return new WaitWhile(() => roll > 0.01 && pitch > 0.01);
		Debug.Log("Выровнялся по крену и тангажу");

		while( Mathf.Abs( (float)accelerometr) > accelerometrThreshold)
		{
			Debug.Log(accelerometr);
			targetPitch += pitchStep;
			throttle -= step;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}

		targetPitch = 0;

		Debug.Log("Завис");
		Debug.Break();
	}
}