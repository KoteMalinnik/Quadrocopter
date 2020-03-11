using UnityEngine;

public class GPS : MonoBehaviour
{
   //Определяем координаты с помощью GPS
	//Определяем горизонтальную и вертикальную скорость по GPS
	public Vector2 coordinates { get; private set; }
	Vector2 previousCoordinates = Vector2.zero;

	public double horizontalSpeed { get; private set; }
	public double verticalSpeed { get; private set; }

	void Update()
	{
		coordinates = new Vector2(transform.position.x, transform.position.y);

		double dt = Time.deltaTime;
		horizontalSpeed = (coordinates.x - previousCoordinates.x) / dt;
		verticalSpeed = (coordinates.y - previousCoordinates.y) / dt;
	}
}
