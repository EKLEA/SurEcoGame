using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudSlotView : MonoBehaviour
{
	public UIItem ItemPrefab;
	
	UIItem uiItem;
	public void SetUpSlot(IReadOnlyInventorySlot slot)
	{
		if(slot.ItemID!=null)
		{
			if(uiItem==null) uiItem=Instantiate(ItemPrefab,transform);
			uiItem.SetUpItem(slot.ItemID,slot.Amount);
		}
		else 
			if(uiItem!=null) DestroyImmediate(uiItem.gameObject);
		
		
	}
}