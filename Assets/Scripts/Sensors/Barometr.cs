using UnityEngine;

public class Barometr : MonoBehaviour
{
	//Определяем высоту с помощью барометра
	//Значение 0 на земеле

    public float hight { get; private set; }
	float previousHight = 0;

	public float verticalSpeed { get; private set; }

	void FixedUpdate()
	{
		hight = transform.position.y;

		float dt = Time.fixedDeltaTime;
		verticalSpeed = (hight - previousHight) / dt;

		previousHight = hight;
	}
}
