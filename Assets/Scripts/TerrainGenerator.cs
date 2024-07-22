using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Terrain")]
public class TerrainGenerator : ScriptableObject
{
	public float baseHeight=8;
	public float grassLayerHeight= 1;
	public NoiseOctaveSettings[] Octaves;
	public NoiseOctaveSettings DomainWarp;
	[Serializable]
	public class NoiseOctaveSettings
	{
		public FastNoiseLite.NoiseType noiseType;
		public float frequency=0.2f;
		public float amplitude=1;
	}
	
	FastNoiseLite[] octaveNoises;
	
	FastNoiseLite warpNoise;
	
	
	public void Init()
	{
		octaveNoises=new FastNoiseLite[Octaves.Length];
		for (int i=0;i<Octaves.Length;i++)
		{
			octaveNoises[i]=new FastNoiseLite();
			octaveNoises[i].SetNoiseType(Octaves[i].noiseType);
			octaveNoises[i].SetFrequency(Octaves[i].frequency);
		}
		warpNoise=new FastNoiseLite();
		warpNoise.SetNoiseType(DomainWarp.noiseType);
		warpNoise.SetFrequency(DomainWarp.frequency);
		warpNoise.SetDomainWarpAmp(DomainWarp.amplitude);
	}
	
	public BlockType[,,] GenerateTerrain(float xOffset,float zOffset)
	{
		var result = new BlockType[ChunkRenderer.chunkWidth,ChunkRenderer.chunkHeight,ChunkRenderer.chunkWidth];
		
		for(int x = 0; x < ChunkRenderer.chunkWidth; x++)
		{
			for(int z = 0; z < ChunkRenderer.chunkWidth;z++)
			{
				//float height =Mathf.PerlinNoise((x+xOffset)*0.2f,(z+zOffset)*0.2f)*2+10;
				float height = GetHeight(x*ChunkRenderer.blockScale+xOffset,z*ChunkRenderer.blockScale+zOffset);
				
				
				for(int y = 0; y<height/ChunkRenderer.blockScale; y++)
				{
					if(height-y*ChunkRenderer.blockScale<grassLayerHeight)
						result[x,y,z]=BlockType.Grass;
					else
					{
						result[x,y,z]= BlockType.Stone;
					}
				}
			}
		}
		return result;
	}
	float GetHeight(float x,float y)
	{
		warpNoise.DomainWarp(ref x, ref y);
		float result = baseHeight;
		for(int i = 0; i < Octaves.Length; i++)
		{
			float noise = octaveNoises[i].GetNoise(x,y);
			result+= noise*Octaves[i].amplitude/2;
		}
		return result;
	}
}
