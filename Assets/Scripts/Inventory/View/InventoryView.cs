using System;
using TMPro;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
	public event Action<InventorySlotView,InventorySlotView> OnSlotsSwitched;
	[SerializeField] InventorySlotView[] _slots;
	[SerializeField] TMP_Text _textOwner;
	
	public string OwnerID
	{
		get=>_textOwner.text;
		set=>_textOwner.text = value;
	}
	
	public InventorySlotView GetInventorySlotView(int index)
	{
		return _slots[index];
	}
	public void OnSlotsSwitchedInvoke(InventorySlotView slot1,InventorySlotView slot2)
	{
		OnSlotsSwitched?.Invoke(slot1,slot2);
	}
}