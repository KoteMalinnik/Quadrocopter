using UnityEngine;

public class GPS : MonoBehaviour
{
   //Определяем координаты с помощью GPS
	//Определяем горизонтальную и вертикальную скорость по GPS
	public Vector3 coordinates { get; private set; }
	Vector3 previousCoordinates = Vector2.zero;

	public Vector3 direction { get; private set; }

	public float horizontalSpeed { get; private set; }
	public Vector2 hSpeedVector { get; private set; }

	void FixedUpdate()
	{
		coordinates = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		float dt = Time.fixedDeltaTime;

		var h1 = (coordinates.x - previousCoordinates.x) / dt;
		var h2 = (coordinates.z - previousCoordinates.z) / dt;

		hSpeedVector = new Vector2(h1, h2);
		horizontalSpeed = Mathf.Sqrt((h1 * h1 + h2 * h2));

		direction = coordinates - previousCoordinates;

		previousCoordinates = coordinates;
	}

	LineRenderer lr_dir;
	void Start()
	{
		lr_dir = gameObject.AddComponent<LineRenderer>();

		lr_dir.startWidth = 0.1f;
		lr_dir.endWidth = 0;
	}

	void Update()
	{
		Vector3[] dir = {transform.position, transform.position + direction*10};
		lr_dir.SetPositions(dir);
	}
}
