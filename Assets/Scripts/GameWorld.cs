using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
	public Dictionary<Vector2Int,ChunkData> ChunksDatas = new Dictionary<Vector2Int, ChunkData>();
	public ChunkRenderer chunkPrefab; 
	public TerrainGenerator terrainGenerator;
	
	public
	Camera mainCamera=> Camera.main;
	Vector2Int currectPlayerChunk;
	public int viewRadius=5;
	
	void Start()
	{
		terrainGenerator.Init();
		Generate();
	}
	void CheckInput()
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
	
	void Update()
	{
		Vector3Int playerWorldPos= Vector3Int.FloorToInt(mainCamera.transform.position/ChunkRenderer.blockScale);
		Vector2Int playerChunk =GetChunkContainingBlock(playerWorldPos);
		if(playerChunk!=currectPlayerChunk	)
		{
			currectPlayerChunk=playerChunk;
			Generate();
		}
		CheckInput();
	}
	void Generate()
	{
		
		for (int x = currectPlayerChunk.x-viewRadius; x< currectPlayerChunk.x+viewRadius;x++)
		{
			for(int y=currectPlayerChunk.y-viewRadius; y< currectPlayerChunk.y+viewRadius;y++)
			{
				Vector2Int chunkDataPosition=new Vector2Int(x,y);
				if(ChunksDatas.ContainsKey(chunkDataPosition)) continue;
				
				LoadAtChunk(chunkDataPosition);
			}
		}
	}
	[ContextMenu("Regenerate terrain")]
	public void Regenerate()
	{
		terrainGenerator.Init();
		foreach( var chunckData in ChunksDatas)
		{
			Destroy(chunckData.Value.chunkRenderer.gameObject);
			
		}
		ChunksDatas.Clear();
		Generate();
	}
	void LoadAtChunk(Vector2Int chunkPosition)
	{
		var xPos= chunkPosition.x*ChunkRenderer.chunkWidth*ChunkRenderer.blockScale;
		var zPos= chunkPosition.y*ChunkRenderer.chunkWidth*ChunkRenderer.blockScale;
				
		ChunkData chunkData=new ChunkData();
		chunkData.chunkPosition= chunkPosition;
		chunkData.blocks=terrainGenerator.GenerateTerrain(xPos,zPos);
		ChunksDatas.Add(chunkPosition, chunkData);
				
		var chunk = Instantiate(chunkPrefab,new Vector3(xPos,0,zPos),Quaternion.identity,transform);
		chunk.chunkData=chunkData;
		chunk.parantWorld=this;
		chunkData.chunkRenderer=chunk;
	}
	public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
	{
		Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x/ChunkRenderer.chunkWidth,blockWorldPos.z/ChunkRenderer.chunkWidth);
		
		if(blockWorldPos.x<0) chunkPosition.x--;
		if(blockWorldPos.z<0) chunkPosition.y--;
		
		return  chunkPosition;
	}
}
