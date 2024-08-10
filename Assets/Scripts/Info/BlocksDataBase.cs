using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BlocksDataBase 
{
	private static Dictionary<BlockType,BlockInfo> blocksCached = new Dictionary<BlockType,BlockInfo>();
	public static void Init()
	{
		blocksCached.Clear();
		Object[] assets = Resources.LoadAll<BlockInfo>("");
		foreach (object obj in assets)
		{
			BlockInfo so = obj as BlockInfo;
			if (!blocksCached.ContainsValue(so))
				blocksCached.Add(so.type, so);
		}
	}
	public static BlockInfo GetInfo(BlockType type)
	{
		//if(blocksCached.Count==0) Init();
		if(blocksCached.TryGetValue(type, out var blockInfo))
		{
			return blockInfo;
		}
		return null;
	}
}

