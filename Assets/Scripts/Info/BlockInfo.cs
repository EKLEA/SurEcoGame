using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Blocks/Normal Block")]
public class BlockInfo : ScriptableObject
{
	public BlockType type;
	public Vector2 PixelsOffset;
	
	public AudioClip StepSound;
	public float TimeToBreak=0.3f;
	
	public virtual Vector2 GetPixelOffset(Vector3Int normal)
	{
		return PixelsOffset;
	}
}

   

