using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
   public void CheckInput()
	{
		if(Input.GetButtonDown("Fire2")|| Input.GetButtonDown("Fire1"))
		{
			bool isDestroying= Input.GetButtonDown("Fire1");
			(string itemID,int Amount)  =EntryPoint.Instance._fastSlotsHudController.GetHudSlotState(PlayerController.Instance.playerState.activeHudSlot);
			var itemInfo =InfoDataBase.ItemsDataBase.GetInfo(itemID);
			Dictionary<Vector3Int,BlockType> blocks=new();
			if(InfoDataBase.ItemsDataBase.GetInfo(itemID).itemType==ItemType.Block)
			{
				
				if(isDestroying)
				{
					for(int x=0;x<PlayerController.Instance.playerState.sizeOfBlockArea.x;x++)
						for(int y=0;y<PlayerController.Instance.playerState.sizeOfBlockArea.y;y++)
							for(int z=0;z<PlayerController.Instance.playerState.sizeOfBlockArea.z;z++)
								blocks[new Vector3Int(x,y,z)]=BlockType.Air;
					PlayerController.Instance.playerState.currentWorld.PlaceBLock(blocks,isDestroying);
				}
				else
				{
					for(int x=0;x<PlayerController.Instance.playerState.sizeOfBlockArea.x;x++)
						for(int y=0;y<PlayerController.Instance.playerState.sizeOfBlockArea.y;y++)
							for(int z=0;z<PlayerController.Instance.playerState.sizeOfBlockArea.z;z++)
								blocks[new Vector3Int(x,y,z)]=(itemInfo as BlockItemInfo).blockType;
				
					PlayerController.Instance.playerState.currentWorld.PlaceBLock(blocks,isDestroying);
				}
			}
			else if(InfoDataBase.ItemsDataBase.GetInfo(itemID).itemType==ItemType.StructureBlock)
			{
				///
			}
				
		}
		
	}
	
}