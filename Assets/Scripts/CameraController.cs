using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
	Vector3 deltaPosition = new Vector3(0, 1, -3);

    void Awake()
    {
        transform.position = target.position + deltaPosition;
		transform.rotation = Quaternion.Euler(20, 0, 0);
    }

    void Update()
    {  
  		transform.position = target.position + deltaPosition;

		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.RotateAround(target.position, Vector3.up, 2);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.RotateAround(target.position, Vector3.up, -2);
		}

		deltaPosition = transform.position - target.position;
    }
}
