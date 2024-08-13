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
}
