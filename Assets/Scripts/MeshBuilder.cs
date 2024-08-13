using System.CodeDom.Compiler;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public static class MeshBuilder
{
	public const int chunkWidth=32;
	public const int chunkWidthSq=chunkWidth*chunkWidth;
	public const int chunkHeight=128;
	public const float blockScale=0.125f;
	
	public static GameWorld.GeneratedMeshData GeneratedMesh(ChunkData chunkData)
	{
		
		List<GameWorld.GeneratedMeshVertex> verticies = new List<GameWorld.GeneratedMeshVertex>();
		int maxY=0;
		for (int y = 0; y < chunkHeight; y++)
		{
			for (int x = 0; x < chunkWidth; x++)
			{
				for (int z= 0; z < chunkWidth; z++)
					if(GenerateBlock(x,y,z,verticies,chunkData))
					{
						if(maxY<y) maxY=y;
					}
			}
		}
		var mesh= new GameWorld.GeneratedMeshData();
		mesh.verticies=verticies.ToArray();
		Vector3 boundsSize=new Vector3(chunkWidth,maxY,chunkWidth)*blockScale;
		mesh.bounds=new Bounds(boundsSize/2,boundsSize);
		mesh.data=chunkData;
		return mesh;
	}
	
	
		
	static bool GenerateBlock(int x, int y, int z,List<GameWorld.GeneratedMeshVertex> verticies,ChunkData chunkData)
	{
		
		var blockPosition = new Vector3Int(x,y,z);
		BlockType blockType=GetBlockAtPosition(blockPosition,chunkData);
		
		if(GetBlockAtPosition(blockPosition,chunkData)==BlockType.Air) return false;
		
		if(GetBlockAtPosition(blockPosition+Vector3Int.right,chunkData)==BlockType.Air) 
		{
			GenerateRightSide(blockPosition,verticies,blockType);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.left,chunkData)==BlockType.Air) 
		{
			GenerateLeftSide(blockPosition,verticies,blockType);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.forward,chunkData)==BlockType.Air) 
		{
			GenerateFrontSide(blockPosition,verticies,blockType);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.back,chunkData)==BlockType.Air) 
		{
			GenerateBackSide(blockPosition,verticies,blockType);
		}
		if(GetBlockAtPosition(blockPosition+Vector3Int.up,chunkData)==BlockType.Air) 
		{
			GenerateTopSide(blockPosition,verticies,blockType);
		}
		if (blockPosition.y>0 && GetBlockAtPosition(blockPosition+Vector3Int.down,chunkData)==BlockType.Air)
		{
			GenerateBottomSide(blockPosition,verticies,blockType);
		}
		return true;
	}
	static BlockType GetBlockAtPosition(Vector3Int blockPosition,ChunkData chunkData)
	{
		if(blockPosition.x >=0 && blockPosition.x < chunkWidth &&
		blockPosition.y >=0 && blockPosition.y < chunkHeight &&
		blockPosition.z >=0 && blockPosition.z < chunkWidth)
		{
			int index =blockPosition.x+blockPosition.y * chunkWidthSq+blockPosition.z*chunkWidth;
			return chunkData.blocks[index];
		}
			
		else
		{
			if(blockPosition.y <0 ||blockPosition.y >= chunkHeight) return BlockType.Air;
			
			
			if(blockPosition.x<0)
			{
				blockPosition.x+=chunkWidth;
				int index =blockPosition.x+blockPosition.y * chunkWidthSq+blockPosition.z*chunkWidth;
				return chunkData.leftChunk.blocks[index];
			}
			if(blockPosition.x>=chunkWidth)
			{
				blockPosition.x-=chunkWidth;
				int index =blockPosition.x+blockPosition.y * chunkWidthSq+blockPosition.z*chunkWidth;
				return chunkData.rightChunk.blocks[index];
			}
			
			if(blockPosition.z<0)
			{
				blockPosition.z+=chunkWidth;
				int index =blockPosition.x + blockPosition.y * chunkWidthSq + blockPosition.z*chunkWidth;
				return chunkData.backChunk.blocks[index];
			}
			if(blockPosition.z>=chunkWidth)
			{
				blockPosition.z-=chunkWidth;
				int index =blockPosition.x+blockPosition.y * chunkWidthSq+blockPosition.z*chunkWidth;
				return chunkData.fwdChunk.blocks[index];
			}
			return BlockType.Air;
		}
	}
	static void GenerateRightSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=sbyte.MaxValue;
		vertex.normalY=0;
		vertex.normalZ=0;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.right,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(1, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
	}
	static void GenerateLeftSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=sbyte.MinValue;
		vertex.normalY=0;
		vertex.normalZ=0;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.left,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(0, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
	}
	static void GenerateFrontSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)	
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=0;
		vertex.normalY=0;
		vertex.normalZ=sbyte.MaxValue;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.forward,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(0, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
	}
	static void GenerateBackSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=0;
		vertex.normalY=0;
		vertex.normalZ=sbyte.MinValue;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.back,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(0, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
	}
	static void GenerateTopSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=0;
		vertex.normalY=sbyte.MaxValue;
		vertex.normalZ=0;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.up,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(0, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 1, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
	}
	
	static void GenerateBottomSide(Vector3Int blockPosition,List<GameWorld.GeneratedMeshVertex> verticies,BlockType blockType)
	{
		GameWorld.GeneratedMeshVertex vertex= new GameWorld.GeneratedMeshVertex();
		vertex.normalX=0;
		vertex.normalY=sbyte.MinValue;
		vertex.normalZ=0;
		vertex.normalW=1;
		GetUvs(blockType,Vector3Int.down,out vertex.uvX,out vertex.uvY);
		
		vertex.pos=(new Vector3(0, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 0, 0) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(0, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
		vertex.pos=(new Vector3(1, 0, 1) + blockPosition)*blockScale;
		verticies.Add(vertex);
		
	}
	static void GetUvs(BlockType blockType, Vector3Int normal,out ushort x,out ushort y)
	{
		BlockInfo blockInfo = InfoDataBase.BlockDataBase.GetInfo(blockType);
		if (blockInfo != null) 
		{
			Vector2 resVec = blockInfo.GetPixelOffset(normal);
			x = (ushort) (resVec.x*256);
			y = (ushort) (resVec.y*256);
		}
		
		
		else
		{
			x=160*256;
			y=224*256;
		}
	}
}


