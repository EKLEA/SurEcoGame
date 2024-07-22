using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Blocks/DB")]
public class BlocksDataBase : ScriptableObject
{
	[SerializeField] private BlockInfo[] Blocks;
	private Dictionary<BlockType,BlockInfo> blocksCached = new Dictionary<BlockType,BlockInfo>();
	void OnEnable()
	{
		blocksCached.Clear();
		foreach (var blockInfo in Blocks)
		{
			blocksCached.Add(blockInfo.type, blockInfo);
		}
	}
	public BlockInfo GetInfo(BlockType type)
	{
		//if(blocksCached.Count==0) Init();
		if(blocksCached.TryGetValue(type, out var blockInfo))
		{
			return blockInfo;
		}
		return null;
	}
}

