using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator 
{
	public static BlockType[,,] GenerateTerrain(int xOffset,int zOffset)
	{
		var result = new BlockType[ChunkRenderer.chunkWidth,ChunkRenderer.chunkHeight,ChunkRenderer.chunkWidth];
		
		for(int x = 0; x < ChunkRenderer.chunkWidth; x++)
		{
			for(int z = 0; z < ChunkRenderer.chunkWidth;z++)
			{
				float height =Mathf.PerlinNoise((x+xOffset)*0.2f,(z+zOffset)*0.2f)*2+10;
				for(int y = 0; y<height; y++)
				{
					result[x,y,z]= BlockType.Grass;
				}
			}
		}
		return result;
	}
}
