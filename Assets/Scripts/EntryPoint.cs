using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
	[SerializeField] ScreenView _screenView;
	[SerializeField] FastSlotsHudView _fastSlotsHudView;
	InventoryService _inventoryService;
	ScreenController _screenController;
	public FastSlotsHudController _fastSlotsHudController;
	private readonly string[] _itemIds={"Stone","Grass"};
	[SerializeField] CameraLogic _camera;
	public static EntryPoint Instance;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
		DontDestroyOnLoad(gameObject);
	}
	void Start()
	{
		
		_screenView.gameObject.SetActive(false);
		_inventoryService=new InventoryService();
		var inventoryDataMy=CreateTestInventory("MY");
		_inventoryService.RegisterInventory(inventoryDataMy);
		
		_screenController=new ScreenController(_inventoryService,_screenView);
		_screenController.OpenInventory("MY");
		
		_fastSlotsHudController=new FastSlotsHudController(_inventoryService,_fastSlotsHudView);
		_fastSlotsHudController.SetUpHud("MY",0);
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
		if(Input.GetKeyDown(KeyCode.I))
		{
			if(!PlayerController.Instance.playerState.isInventoryOpen)
			{
				PlayerController.Instance.playerState.isInventoryOpen=true;
				_screenView.gameObject.SetActive(true);
				Cursor.lockState=CursorLockMode.None;
				_camera.enabled=false;
				
			}
			else
			{
				if(!PlayerController.Instance.playerState.isDragging)
				{
					PlayerController.Instance.playerState.isInventoryOpen=false;
					_screenView.gameObject.SetActive(false);
					Cursor.lockState=CursorLockMode.Locked;
					_camera.enabled=true;
				}
			}
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
