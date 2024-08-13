using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventorySlotController 
{
	readonly InventorySlotView _view;
	public InventorySlotController(IReadOnlyInventorySlot slot, InventorySlotView view)//вьюшку можно переделать под интерфейс
	{
		_view=view;
		
		slot.ItemIDChanged+=OnSlotItemIDChanged;
		slot.ItemAmountChanged+=OnSlotAmountChanged;
		
		view.Title=slot.ItemID;// менять в значение локализация
		view.Amount=slot.Amount;
	}

	private void OnSlotAmountChanged(int newAmount)
	{
		_view.Amount=newAmount;
		_view.RefreshSlot();
	}

	private void OnSlotItemIDChanged(string newItemID)
	{
		_view.Title=newItemID;
		_view.RefreshSlot();
	}
}