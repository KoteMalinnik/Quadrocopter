using UnityEngine;

public class GPS : MonoBehaviour
{
   //Определяем координаты с помощью GPS
	//Определяем горизонтальную и вертикальную скорость по GPS
	public Vector3 coordinates { get; private set; }
	Vector3 previousCoordinates = Vector2.zero;

	public Vector3 direction { get; private set; }

	public float horizontalSpeed { get; private set; }
	public float horSpeedX { get; private set; }
	public float horSpeedZ { get; private set;}

	void FixedUpdate()
	{
		coordinates = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		float dt = Time.fixedDeltaTime;

		horSpeedX = (coordinates.x - previousCoordinates.x) / dt;
		horSpeedZ = (coordinates.z - previousCoordinates.z) / dt;

		horizontalSpeed = Mathf.Sqrt((horSpeedX * horSpeedX + horSpeedZ * horSpeedZ));

		direction = coordinates - previousCoordinates;

		previousCoordinates = coordinates;
	}

	LineRenderer lr_dir;
	void Start()
	{
		lr_dir = gameObject.AddComponent<LineRenderer>();

		lr_dir.startWidth = 0.1f;
		lr_dir.endWidth = 0.1f;
	}

	void Update()
	{
		Vector3[] dir = {transform.position, transform.position + direction*10};
		lr_dir.SetPositions(dir);
	}
}
