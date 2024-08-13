using UnityEngine;

public class ScreenController 
{
	readonly InventoryService _inventoryService;
	readonly ScreenView _view;
	
	InventoryGridController _currentInventoryController;
	
	public ScreenController(InventoryService inventoryService,ScreenView view)
	{
		_inventoryService=inventoryService;
		_view=view;
	}
	public void OpenInventory(string OwnerID)
	{
		//здесь сделать iDisposable при открытии др инвентаря.
		var inventory = _inventoryService.GetInventory(OwnerID);
		var inventoryView=_view.inventoryView;
		_currentInventoryController=new InventoryGridController(inventory,inventoryView);
	}
}