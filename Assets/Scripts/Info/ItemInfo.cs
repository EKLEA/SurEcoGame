using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Game/Item/Simple Item")]
public class ItemInfo : ScriptableObject
{
	public string ItemID;
	public Vector2Int PixelsOffset;
	public int SizeOfTexture;
	public int maxInSlot;
	public ItemType itemType;
	
}
public enum ItemType :byte
{
	SimpleItem=0,
	Block=1,
	StructureBlock=2,
	UsableItem=3,
	
}
