using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
	public Dictionary<Vector2Int,ChunkData> ChunksDatas = new Dictionary<Vector2Int, ChunkData>();
	public ChunkRenderer chunkPrefab;
	Camera mainCamera=> Camera.main;
	
	void Start()
	{
		for (int x = 0; x<1;x++)
		{
			for(int y=0;y<1;y++)
			{
				var xPos= (int)(x*ChunkRenderer.chunkWidth*ChunkRenderer.blockScale);
				var zPos= (int)(y*ChunkRenderer.chunkWidth*ChunkRenderer.blockScale);
				
				ChunkData chunkData=new ChunkData();
				chunkData.chunkPosition= new Vector2Int(x,y);
				chunkData.blocks=TerrainGenerator.GenerateTerrain(xPos,zPos);
				ChunksDatas.Add(new Vector2Int(x,y), chunkData);
				
				var chunk = Instantiate(chunkPrefab,new Vector3(xPos,0,zPos),Quaternion.identity,transform);
				chunk.chunkData=chunkData;
				chunk.parantWorld=this;
				chunkData.chunkRenderer=chunk;
			}
		}
	}
	
	void Update()
	{
		if(Input.GetButtonDown("Fire2")|| Input.GetButtonDown("Fire1"))
		{
			bool isDestroying= Input.GetButtonDown("Fire1");
			Ray  ray = mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));
			
			if(Physics.Raycast(ray, out var hitInfo,PlayerController.Instance.maxDistanceOfItecation,PlayerController.Instance.playerMovement.groundLayer))
			{
				Vector3 blockCenter;
				if(isDestroying)
					blockCenter=hitInfo.point-hitInfo.normal*ChunkRenderer.blockScale/2;
				else
					blockCenter=hitInfo.point+hitInfo.normal*ChunkRenderer.blockScale/2;
					
				Vector3Int blockWorldPos= Vector3Int.FloorToInt(blockCenter/ChunkRenderer.blockScale);
				Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
				if(ChunksDatas.TryGetValue(chunkPos, out var chunkData))
				{
					Vector3Int chunkOrigin = new Vector3Int(chunkPos.x,0,chunkPos.y)*ChunkRenderer.chunkWidth;
					if(isDestroying)
						chunkData.chunkRenderer.DestroyBlock(blockWorldPos-chunkOrigin);
					else
						chunkData.chunkRenderer.SpawnBlock(blockWorldPos-chunkOrigin);
				}
			}
		}	
	}
	public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
	{
		return  new Vector2Int(blockWorldPos.x/ChunkRenderer.chunkWidth,blockWorldPos.z/ChunkRenderer.chunkWidth);
	}
}
