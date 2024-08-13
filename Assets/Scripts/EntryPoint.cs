using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
	[SerializeField] ScreenView _screenView;
	InventoryService _inventoryService;
	ScreenController _screenController;
	private readonly string[] _itemIds={"Stone","Grass"};
	void Start()
	{
		_inventoryService=new InventoryService();
		var inventoryDataMy=CreateTestInventory("MY");
		_inventoryService.RegisterInventory(inventoryDataMy);
		
		_screenController=new ScreenController(_inventoryService,_screenView);
		_screenController.OpenInventory("MY");
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			var rIndex=Random.Range(0, _itemIds.Length);
			var rItemID=_itemIds[rIndex];
			var rAmount=Random.Range(0, 50);
			var result=_inventoryService.AddItems("MY",rItemID,rAmount);
			Debug.Log($"Items Added: ${rItemID}. Amount:{result.ItemsAddedAmount}");
		}
	}
	private InventoryGridData CreateTestInventory(string ownerID)
	{
		var size =new Vector2Int(4,9);
		var createdInventorySlots=new List<InventorySlotData>();
		var length=size.x*size.y;// вытасикивать из конфигов по типу энум
		for (int i=0; i<length; i++)
			createdInventorySlots.Add(new InventorySlotData());
		var createdInventoryData=new InventoryGridData
		{
			OwnerID = ownerID,
			Size=size,
			Slots=createdInventorySlots
		};
		return createdInventoryData;
	}
}
