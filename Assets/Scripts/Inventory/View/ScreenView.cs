using UnityEngine;

public class ScreenView : MonoBehaviour
{
	[SerializeField] InventoryView _inventoryView;
	public InventoryView inventoryView=> _inventoryView;	
}