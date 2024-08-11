using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryService
{
	private readonly Dictionary<string,InventoryGrid> _inventoryMap=new();
	
	public InventoryGrid RegisterInventory(InventoryGridData inventoryGridData)
	{
		var inventory = new InventoryGrid(inventoryGridData);
		_inventoryMap[inventory.OwnerID] = inventory;
		
		return inventory;
	}
	
	public AddItemsToInventoryGridResult AddItems(string OwnerID, string itemID, int amount=1)
	{
		var inventory = _inventoryMap[OwnerID];
		return inventory.AddItems(itemID,amount);
	}
	public AddItemsToInventoryGridResult AddItemsTo(string OwnerID, Vector2Int slotCoords, string itemID, int amount=1)
	{
		var inventory = _inventoryMap[OwnerID];
		return inventory.AddItems(slotCoords,itemID,amount);
	}
	public RemoveItemsFromInventoryGridResult RemoveItemsFromInventory(string OwnerID, string itemID, int amount=1)
	{
		var inventory = _inventoryMap[OwnerID];
		return inventory.RemoveItems(itemID,amount);
	}
	public RemoveItemsFromInventoryGridResult RemoveItemsFromInventory(string OwnerID,Vector2Int slotCoords, string itemID, int amount=1)
	{
		var inventory = _inventoryMap[OwnerID];
		return inventory.RemoveItems(slotCoords, itemID, amount);
	}
	public bool Has(string OwnerID,string itemID,int amount=1)
	{
		var inventory = _inventoryMap[OwnerID];
		return inventory.Has(itemID, amount);
	}
	public IReadOnlyInventoryGrid GetInventory(string OwnerID)
	{
		return _inventoryMap[OwnerID];
	}
}