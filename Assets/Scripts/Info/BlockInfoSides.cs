using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Blocks/Sides Block")]
public class BlockInfoSides : BlockInfo
{
	public Vector2 PixelsOffsetUp;
	public Vector2 PixelsOffsetDown;
	public override Vector2 GetPixelOffset(Vector3Int normal)
	{
		if(normal==Vector3Int.up) return PixelsOffsetUp;
		else if(normal==Vector3Int.down) return PixelsOffsetDown;
		else return PixelsOffset;
	}

}
