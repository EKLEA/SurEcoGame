using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FastSlotsHudController 
{
	InventoryService _inventoryService;
	FastSlotsHudView _view;
	List<FastSlotController> _slotsController = new();
	int _indexOfLine;
	InventoryGrid _inventory;
	public FastSlotsHudController(InventoryService inventoryService,FastSlotsHudView view)
	{
		_inventoryService=inventoryService;
		_view=view;
	}
	public void SetUpHud(string OwnerID,int indexOfLine)
	{
		_inventory = _inventoryService.GetInventory(OwnerID) as InventoryGrid;
		_indexOfLine=indexOfLine;
		IReadOnlyInventorySlot[,] slots = _inventory.GetSlots();
		if (_indexOfLine>_inventory.Size.x||_indexOfLine<0) return;
		for (int i=0; i<_inventory.Size.y;i++)
			_slotsController.Add(new FastSlotController(slots[_indexOfLine,i],_view.hudSlots[i]));
	}
	public (string,int) GetHudSlotState(int index)
	{
		return _slotsController[index].GetSlotState();
	}
	public void RemoveItemsFromActiveSlot(int activeSlot,int amount)
	{
		_inventory.RemoveItems(new Vector2Int(_indexOfLine,activeSlot),_inventory.GetSlots()[_indexOfLine,activeSlot].ItemID,amount);
		
	}
}