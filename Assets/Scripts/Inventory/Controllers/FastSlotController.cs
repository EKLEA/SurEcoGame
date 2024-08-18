using System;
using UnityEngine;

public class  FastSlotController
{
	HudSlotView _view;
	IReadOnlyInventorySlot _slot;
	public FastSlotController (IReadOnlyInventorySlot slot, HudSlotView view)
	{
		_view=view;
		_slot=slot;
		
		slot.ItemIDChanged+=OnSlotItemIDChanged;
		slot.ItemAmountChanged+=OnSlotAmountChanged;
		
		view.SetUpSlot(slot);
	}
	public (string, int) GetSlotState()
	{
		return (_slot.ItemID, _slot.Amount);
	}
	private void OnSlotAmountChanged(int newAmount)
	{
		_view.SetUpSlot(_slot);
	}

	private void OnSlotItemIDChanged(string newItemID)
	{
		_view.SetUpSlot(_slot);
	}
}