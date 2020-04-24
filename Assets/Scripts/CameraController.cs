using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("General")]
	public float sensitivity = 2; // чувствительность мышки
	public float distance = 5; // расстояние между камерой и игроком
	public float height = 2.3f; // высота

	[Header("Clamp Angle")]
	public float minY = 15f; // ограничение углов при наклоне
	public float maxY = 15f;

	float rotationY;
	public Transform player;

	void Start()
	{
		gameObject.tag = "MainCamera";
	}

	void Update()
	{
		if (player)
		{
			// вращение камеры вокруг игрока
			transform.RotateAround(player.position, Vector3.up, Input.GetAxis("Mouse X") * sensitivity);

			// определяем точку на указанной дистанции от игрока
			Vector3 position = player.position - (transform.rotation * Vector3.forward * distance);
			position = new Vector3(position.x, player.position.y + height, position.z); // корректировка высоты

			// поворот камеры по оси Х
			rotationY += Input.GetAxis("Mouse Y") * sensitivity;
			rotationY = Mathf.Clamp(rotationY, -Mathf.Abs(minY), Mathf.Abs(maxY));
			transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);

			transform.position = position;
		}
	}
}