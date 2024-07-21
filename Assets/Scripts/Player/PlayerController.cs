using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public PlayerMovement playerMovement=>GetComponent<PlayerMovement>();
	public float minDistanceOfItecation=0;
	public float maxDistanceOfItecation=5;
	public static PlayerController Instance;
	 private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
		DontDestroyOnLoad(gameObject);
	}
	
	void Start()
	{
		
	}
	void Update()
	{
		Vector3 moveVec= transform.right*Input.GetAxis("Horizontal") + transform.forward* Input.GetAxis("Vertical");
		playerMovement.Move(moveVec);
		if(Input.GetButtonDown("Jump"))
			playerMovement.JumpMoment();
	}
}
