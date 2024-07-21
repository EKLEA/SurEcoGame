using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MoveHandler
{
	
	
	public float groundCheckX;
	public float groundCheckY;
	public Transform groundCheckPoint;
	public LayerMask groundLayer;
	
	public new void Move(Vector3 moveVec)
	{
		rb.velocity=new Vector3(moveVec.x*speed,rb.velocity.y,moveVec.z*speed);
	}
	public new void JumpMoment()
	{
		if(Grounded())
			rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
	}
	
	
	
	public new bool Grounded()
	{
		return 
		(
			Physics.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) ||
			Physics.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer) ||
			Physics.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
		);
		   
	}
}
