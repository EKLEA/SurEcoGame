using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Game/Terrain")]
public class TerrainGenerator : ScriptableObject
{
	public float baseHeight=4;
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
	
	public BlockType[] GenerateTerrain(float xOffset,float zOffset)
	{
		var result = new BlockType[MeshBuilder.chunkWidth*MeshBuilder.chunkHeight*MeshBuilder.chunkWidth];
		
		for(int x = 0; x < MeshBuilder.chunkWidth; x++)
		{
			for(int z = 0; z < MeshBuilder.chunkWidth;z++)
			{
				float worldX = x * MeshBuilder.blockScale+xOffset;
				float worldZ = z * MeshBuilder.blockScale+zOffset;
				
				float height = GetHeight(worldX,worldZ);
				
				
				for(int y = 0; y<height/MeshBuilder.blockScale; y++)
				{
					int index =x+y * MeshBuilder.chunkWidthSq+z*MeshBuilder.chunkWidth;
					
					if(height-y*MeshBuilder.blockScale<grassLayerHeight)
						result[index]=BlockType.Grass;
					else
					{
						result[index]= BlockType.Stone;
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
