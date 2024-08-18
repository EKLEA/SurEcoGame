using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotView : MonoBehaviour,IDropHandler
{
	public DraggableItem ItemPrefab;
	[HideInInspector] public DraggableItem InventoryItem;
	[HideInInspector] public string Title;
	[HideInInspector]public int Amount;

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("111");
		GameObject dropped=eventData.pointerDrag;
		DraggableItem draggableItem=dropped.GetComponent<DraggableItem>();
		var tempGM=draggableItem.parentAfterDrag.GetComponent<InventorySlotView>();
		
		if(transform.childCount>0)
		{
			draggableItem.parentAfterDrag.GetComponent<InventorySlotView>().InventoryItem=InventoryItem;
			transform.GetChild(0).SetParent(draggableItem.parentAfterDrag);
			
		}
		draggableItem.parentAfterDrag=transform;
		InventoryItem=draggableItem;
		GetComponentInParent<InventoryView>().OnSlotsSwitchedInvoke(tempGM,this);
		PlayerController.Instance.playerState.isDragging=false;
	}
	
	
	public void RefreshSlot()
	{
		if(Title==null)
		{
			if(InventoryItem!=null)
			{
				DestroyImmediate(InventoryItem.gameObject);
				InventoryItem=null;
			}
		}
		else
		{
			if(InventoryItem==null)
			{
				InventoryItem = Instantiate(ItemPrefab,transform);
				InventoryItem.transform.localScale = Vector3.one;
				InventoryItem.transform.localPosition = Vector3.zero;
				InventoryItem.transform.SetAsFirstSibling();
			}
			else
				InventoryItem.SetUpItem(Title,Amount);
		}
	}
}