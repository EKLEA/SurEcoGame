using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
	public GameWorld currentWorld;
	public Vector3Int sizeOfBlockArea;
	public bool isDragging=false;
	public bool isInventoryOpen=false;
	public int activeHudSlot;
}
