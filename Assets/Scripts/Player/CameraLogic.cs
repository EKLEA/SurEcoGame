using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
	public float mouseSensitivity=100f;
	public Transform playerBody;
	float xRotation=0f;
	void Start ()
	{
		Cursor.lockState=CursorLockMode.Locked;
	}
	void Update()
	{
		float xMouse= Input.GetAxis("Mouse X")*mouseSensitivity*Time.deltaTime;
		float yMouse= Input.GetAxis("Mouse Y")*mouseSensitivity*Time.deltaTime;
		
		xRotation-=yMouse;
		xRotation= Math.Clamp(xRotation,-90f,90f);
		
		transform.localRotation=Quaternion.Euler(xRotation,0f,0f);
		playerBody.Rotate(Vector3.up*xMouse);
	}
}
