using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadOnlyInventoryGrid: IReadOnlyInventory
{
	event Action<Vector2Int> SizeChanged;
	Vector2Int Size{get;}
	
	IReadOnlyInventorySlot[,] GetSlots();
}