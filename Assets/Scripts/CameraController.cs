using UnityEngine;

public class CameraController : MonoBehaviour
{
	public enum InversionX { Disabled = 0, Enabled = 1 };
	public enum InversionY { Disabled = 0, Enabled = 1 };
	public enum Smooth { Disabled = 0, Enabled = 1 };

	[Header("General")]
	public float sensitivity = 2; // чувствительность мышки
	public float distance = 5; // расстояние между камерой и игроком
	public float height = 2.3f; // высота

	[Header("Clamp Angle")]
	public float minY = 15f; // ограничение углов при наклоне
	public float maxY = 15f;

	[Header("Invert")] // инверсия осей
	public InversionX inversionX = InversionX.Disabled;
	public InversionY inversionY = InversionY.Disabled;

	[Header("Smooth Movement")]
	public Smooth smooth = Smooth.Enabled;
	public float speed = 8; // скорость сглаживания

	float rotationY;
	int inversY, inversX;
	public Transform player;

	void Start()
	{
		gameObject.tag = "MainCamera";
	}

	void FixedUpdate()
	{
		if (player)
		{
			if (inversionX == InversionX.Disabled) inversX = 1; else inversX = -1;
			if (inversionY == InversionY.Disabled) inversY = -1; else inversY = 1;

			// вращение камеры вокруг игрока
			transform.RotateAround(player.position, Vector3.up, Input.GetAxis("Mouse X") * sensitivity * inversX);

			// определяем точку на указанной дистанции от игрока
			Vector3 position = player.position - (transform.rotation * Vector3.forward * distance);
			position = new Vector3(position.x, player.position.y + height, position.z); // корректировка высоты

			// поворот камеры по оси Х
			rotationY += Input.GetAxis("Mouse Y") * sensitivity;
			rotationY = Mathf.Clamp(rotationY, -Mathf.Abs(minY), Mathf.Abs(maxY));
			transform.localEulerAngles = new Vector3(rotationY * inversY, transform.localEulerAngles.y, 0);

			transform.position = position;
		}
	}
}