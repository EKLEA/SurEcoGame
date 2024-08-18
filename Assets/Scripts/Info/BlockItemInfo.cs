using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item/Block Item")]
public class BlockItemInfo : ItemInfo
{ 
   public BlockType blockType=> (BlockType)Enum.Parse(typeof(BlockType), ItemID);	
}