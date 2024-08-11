using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public readonly struct RemoveItemsFromInventoryGridResult
{
	public readonly string InventoryOwnerID;
	public readonly int ItemsToRemoveAmount;
	public readonly bool Success;
	
	public RemoveItemsFromInventoryGridResult(string inventoryOwnerID, int itemsToRemoveAmount,bool success)
	{	
		InventoryOwnerID=inventoryOwnerID;
		ItemsToRemoveAmount = itemsToRemoveAmount;
		Success = success;
	}
}