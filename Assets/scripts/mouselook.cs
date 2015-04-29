using UnityEngine;
using System.Collections;

public class mouselook : MonoBehaviour {

	public GameObject player;

	public float cameraXOffset = 0;
	public float cameraYOffset = 0.8f;
	public float cameraDistanceFromPlayer = 3.5f;

	private float yRotation = 0; 
	private float xRotation = 0; 
	private Quaternion RotationAngluar;

	private float aroundXrotation = 0;
	private float aroundYrotation = 0;
	private Quaternion cameraaround;

	void Start()
	{
		Screen.lockCursor = true;
	}

	void Update () 
	{

		float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
		if (scrollwheel != 0)
		{
			cameraDistanceFromPlayer -= scrollwheel * 5;
		}

		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			aroundXrotation = xRotation;
			aroundYrotation = yRotation;
		}

		if (!Input.GetKey(KeyCode.Mouse1))
		{
			yRotation += Input.GetAxis("Mouse X") ; 
			xRotation -= Input.GetAxis("Mouse Y") ; 
			
			xRotation = Mathf.Clamp(xRotation, -10, 30); 
			RotationAngluar = Quaternion.Euler(xRotation, yRotation, 0);
			transform.rotation = RotationAngluar;
		}
		else
		{
			aroundXrotation -= Input.GetAxis("Mouse Y"); 
			aroundYrotation += Input.GetAxis("Mouse X"); 
			
			aroundXrotation = Mathf.Clamp(aroundXrotation, -10, 30);
			cameraaround = Quaternion.Euler(aroundXrotation, aroundYrotation, 0);
			transform.rotation = cameraaround;
		}
	}

	void LateUpdate()
	{
		if (!Input.GetKey(KeyCode.Mouse1))
		{
			transform.position = RotationAngluar * new Vector3(cameraXOffset, cameraYOffset, -cameraDistanceFromPlayer) + player.transform.position;
		}
		else
		{
			transform.position = cameraaround * new Vector3(cameraXOffset, cameraYOffset, -cameraDistanceFromPlayer) + player.transform.position;
		}

	}
}


