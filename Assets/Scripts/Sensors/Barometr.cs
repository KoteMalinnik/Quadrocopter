using UnityEngine;

public class Barometr : MonoBehaviour
{
	//Определяем высоту с помощью барометра
	//Значение 0 на земеле

    public double hight { get; private set; }

	void Update()
	{
		hight = transform.position.y;
	}
}
