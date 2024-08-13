using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class InventoryGridController
{
	readonly List<InventorySlotController> _slotController=new();// публик сделать если будет меняться размер инвентаря
	IReadOnlyInventoryGrid _inventoryGrid;
	public InventoryGridController(IReadOnlyInventoryGrid inventory,InventoryView view)
	{
		var size = inventory.Size;
		var slots = inventory.GetSlots();
		_inventoryGrid=inventory;
		view.OnSlotsSwitched+=SwitchViewSlots;
		var lineLength=size.y;
		
		for(int i=0; i<size.x; i++)
		{
			for(int j=0; j<size.y; j++)
			{
				var index=i*lineLength+j;
				var slotView=view.GetInventorySlotView(index);
				var slot=slots[i,j];
				_slotController.Add(new InventorySlotController(slot,slotView));
			}
		}
		view.OwnerID=inventory.OwnerID;
	}
	void SwitchViewSlots(InventorySlotView slot1, InventorySlotView slot2)
	{
		var exInv=_inventoryGrid as InventoryGrid;
		int indexA=slot1.transform.GetSiblingIndex();
		int indexB=slot2.transform.GetSiblingIndex();
		exInv.SwitchSlots(new Vector2Int(indexA/exInv.Size.y,indexA%exInv.Size.y),new Vector2Int(indexB/exInv.Size.y,indexB%exInv.Size.y));
	}
}