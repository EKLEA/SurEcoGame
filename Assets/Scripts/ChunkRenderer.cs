using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer),typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
	public const int chunkWidth=32;
	public const int chunkHeight=128;
	public const float blockScale=0.125f;
	public GameWorld parantWorld;
	public BlocksDataBase Blocks;
	
	Mesh chunkMesh;
	public ChunkData chunkData;
	private List<Vector3> verticies = new List<Vector3>();
	private List<Vector2> uvs = new List<Vector2>();
	private List<int> triangles = new List<int>();


	private void Start() 
	{
		chunkMesh = new Mesh();

		RegenerateMesh();

		GetComponent<MeshFilter>().mesh = chunkMesh;
	}
	void RegenerateMesh()
	{
		verticies.Clear();
		uvs.Clear();
		triangles.Clear();
		for (int x = 0; x < chunkWidth; x++)
		{
			for (int z = 0; z < chunkWidth; z++)
			{
				for (int y = 0; y < chunkHeight; y++)
					GenerateBlock(x,y,z);
			}
		}
		
		chunkMesh.triangles = Array.Empty<int>();
		chunkMesh.vertices = verticies.ToArray();
		chunkMesh.uv = uvs.ToArray();
		chunkMesh.triangles = triangles.ToArray();
		chunkMesh.Optimize();

		chunkMesh.RecalculateNormals();
		chunkMesh.RecalculateBounds();
		
		
		GetComponent<MeshCollider>().sharedMesh = chunkMesh;
	}
	public void SpawnBlock(Vector3Int blockPostition)
	{
		chunkData.blocks[blockPostition.x,blockPostition.y,blockPostition.z] = BlockType.Stone;
		RegenerateMesh();
	}
	
	public void DestroyBlock(Vector3Int blockPostition)
	{
		chunkData.blocks[blockPostition.x,blockPostition.y,blockPostition.z] = BlockType.Air;
		RegenerateMesh();
	}
	private void GenerateBlock(int x, int y, int z)
	{
		
		var blockPosition = new Vector3Int(x,y,z);
		BlockType blockType=GetBlockAtPosition(blockPosition);
		
		if(GetBlockAtPosition(blockPosition)==BlockType.Air) return;
		
		if(GetBlockAtPosition(blockPosition+Vector3Int.right)==BlockType.Air) 
		{
			GenerateRightSide(blockPosition);
			AddUvs(blockType,Vector3Int.right);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.left)==BlockType.Air) 
		{
			GenerateLeftSide(blockPosition);
			AddUvs(blockType,Vector3Int.left);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.forward)==BlockType.Air) 
		{
			GenerateFrontSide(blockPosition);
			AddUvs(blockType,Vector3Int.forward);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.back)==BlockType.Air) 
		{
			GenerateBackSide(blockPosition);
			AddUvs(blockType,Vector3Int.back);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.up)==BlockType.Air) 
		{
			GenerateTopSide(blockPosition);
			AddUvs(blockType,Vector3Int.up);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.down)==BlockType.Air)
		{
			GenerateBottomSide(blockPosition);
			AddUvs(blockType,Vector3Int.down);
		}
	}
	BlockType GetBlockAtPosition(Vector3Int blockPosition)
	{
		if(blockPosition.x >=0 && blockPosition.x < chunkWidth &&
		blockPosition.y >=0 && blockPosition.y < chunkHeight &&
		blockPosition.z >=0 && blockPosition.z < chunkWidth)
			return chunkData.blocks[blockPosition.x, blockPosition.y, blockPosition.z];
		else
		{
			if(blockPosition.y <0 ||blockPosition.y >= chunkWidth) return BlockType.Air;
			
			Vector2Int adjacentChunkPosition = chunkData.chunkPosition;
			if(blockPosition.x<0)
			{
				adjacentChunkPosition.x--;
				blockPosition.x+=chunkWidth;
			}
			else if(blockPosition.x>=chunkWidth)
			{
				adjacentChunkPosition.x++;
				blockPosition.x-=chunkWidth;
			}
			
			if(blockPosition.z<0)
			{
				adjacentChunkPosition.y--;
				blockPosition.z+=chunkWidth;
			}
			else if(blockPosition.z>=chunkWidth)
			{
				adjacentChunkPosition.y++;
				blockPosition.z-=chunkWidth;
			}
			
			if(parantWorld.ChunksDatas.TryGetValue(adjacentChunkPosition,out ChunkData adjacentChunk))
			{
				return adjacentChunk.blocks[blockPosition.x, blockPosition.y,blockPosition.z];
			}
			else
				return BlockType.Air;
		}
	}
	void GenerateRightSide(Vector3Int blockPosition)
	{
		verticies.Add((new Vector3(1, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 0, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 1) + blockPosition)*blockScale);

		AddLastVerticiesSquare();
	}
	void GenerateLeftSide(Vector3Int blockPosition)
	{
		verticies.Add((new Vector3(0, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 0, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 1, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 1, 1) + blockPosition)*blockScale);
		

		AddLastVerticiesSquare();
	}
	void GenerateFrontSide(Vector3Int blockPosition)	
	{
		verticies.Add((new Vector3(0, 0, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 0, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 1, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 1) + blockPosition)*blockScale);


		AddLastVerticiesSquare();
	}
	void GenerateBackSide(Vector3Int blockPosition)
	{
		verticies.Add((new Vector3(0, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 1, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 0) + blockPosition)*blockScale);
		AddLastVerticiesSquare();
	}
	void GenerateTopSide(Vector3Int blockPosition)
	{
		verticies.Add((new Vector3(0, 1, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 1, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 1, 1) + blockPosition)*blockScale);
		AddLastVerticiesSquare();
	}
	
	void GenerateBottomSide(Vector3Int blockPosition)
	{
		verticies.Add((new Vector3(0, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 0, 0) + blockPosition)*blockScale);
		verticies.Add((new Vector3(0, 0, 1) + blockPosition)*blockScale);
		verticies.Add((new Vector3(1, 0, 1) + blockPosition)*blockScale);
		AddLastVerticiesSquare();
	}
	
	void AddLastVerticiesSquare()
	{
		
		
		triangles.Add(verticies.Count-4);
		triangles.Add(verticies.Count-3);
		triangles.Add(verticies.Count-2);


		triangles.Add(verticies.Count-3);
		triangles.Add(verticies.Count-1);
		triangles.Add(verticies.Count-2);
	}
	void AddUvs(BlockType blockType,Vector3Int normal)
	{
		Vector2 uv;
		BlockInfo info = Blocks.GetInfo(blockType);
		if(info!=null)
		{
			uv= info.GetPixelOffset(normal)/256;
		}
		
		else
		{
			uv = new Vector2(160f/256,224f/256);
		}
		for(int i = 0; i < 4; i++)
		{
			uvs.Add(uv);
		}
	}
}

