using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class GameWorld : MonoBehaviour
{
	public Dictionary<Vector2Int,ChunkData> ChunksDatas = new Dictionary<Vector2Int, ChunkData>();
	
	public ChunkRenderer chunkPrefab; 
	public TerrainGenerator terrainGenerator;
	
	
	Camera mainCamera=> Camera.main;
	Vector2Int currentPlayerChunk;
	public int viewRadius=5;
	ConcurrentQueue<GeneratedMeshData> meshingResult = new 	ConcurrentQueue<GeneratedMeshData>();
	public float viewPow=90;
	
	
	void Start()
	{
		BlocksDataBase.Init();
		ChunkRenderer.InitTriangles();
		terrainGenerator.Init();
		StartCoroutine(Generate(false));
	}
	void CheckInput()
	{
		if(Input.GetButtonDown("Fire2")|| Input.GetButtonDown("Fire1"))
		{
			bool isDestroying= Input.GetButtonDown("Fire1");
			Ray  ray = mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));
			
			if(Physics.Raycast(ray, out var hitInfo,PlayerController.Instance.maxDistanceOfItecation))
			{
				Vector3 blockCenter;
				if(isDestroying)
					blockCenter=hitInfo.point-hitInfo.normal*MeshBuilder.blockScale/2;
				else
					blockCenter=hitInfo.point+hitInfo.normal*MeshBuilder.blockScale/2;
					
				Vector3Int blockWorldPos= Vector3Int.FloorToInt(blockCenter/MeshBuilder.blockScale);
				
				
				Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
				
				
				if(ChunksDatas.TryGetValue(chunkPos, out var chunkData))
				{
					Vector3Int chunkOrigin = new Vector3Int(chunkPos.x*MeshBuilder.chunkWidth,0,chunkPos.y*MeshBuilder.chunkWidth);
					if(isDestroying)
						chunkData.chunkRenderer.DestroyBlock(blockWorldPos-chunkOrigin);
					else
						chunkData.chunkRenderer.SpawnBlock(blockWorldPos -chunkOrigin);
				}
			}
		}	
	}
	
	void Update()
	{
		Vector3Int playerWorldPos= Vector3Int.FloorToInt(mainCamera.transform.position/MeshBuilder.blockScale);
		Vector2Int playerChunk =GetChunkContainingBlock(playerWorldPos);
		
		if(playerChunk!=currentPlayerChunk	)
		{
			currentPlayerChunk=playerChunk;
			StartCoroutine(Generate(true));
		}
		CheckInput();
		
		if(meshingResult.TryDequeue(out var meshData))
		{
			var xPos= meshData.data.chunkPosition.x*MeshBuilder.chunkWidth*MeshBuilder.blockScale;
			var zPos=  meshData.data.chunkPosition.y*MeshBuilder.chunkWidth*MeshBuilder.blockScale;
			if ( meshData.data.State== ChunkDataState.SpawnedInWorld) return;
			var chunk = Instantiate(chunkPrefab,new Vector3(xPos,0,zPos),Quaternion.identity,transform);
			chunk.chunkData= meshData.data;
			chunk.parentWorld=this;
			
			
			chunk.SetMesh(meshData);
			
			meshData.data.chunkRenderer=chunk;
			meshData.data.State= ChunkDataState.SpawnedInWorld;
		}
	}
	IEnumerator Generate(bool wait)
	{
		int loadRadius = viewRadius + 5;
		Vector2Int center = currentPlayerChunk;
		List<ChunkData> loadingChunks = new List<ChunkData>();
		for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
		{
			for (int y = center.y - loadRadius; y <= center.y + loadRadius; y++)
			{
				Vector2Int chunkPosition = new Vector2Int(x, y);

				if (ChunksDatas.ContainsKey(chunkPosition)) continue;

				ChunkData loadingChunk = LoadChunkAt(chunkPosition);
				loadingChunks.Add(loadingChunk);
				if (wait) yield return null;
			}
		}

		while (loadingChunks.Any(c => c.State == ChunkDataState.StartedLoading))
		{
			yield return null;
		}
		
		for (int x = center.x - viewRadius; x <= center.x + viewRadius; x++)
		{
			for (int y = center.y - viewRadius; y <= center.y + viewRadius; y++)
			{
				Vector2Int chunkPosition = new Vector2Int(x, y);
				ChunkData chunkData = ChunksDatas[chunkPosition];
				if ((x-currentPlayerChunk.x)*(x-currentPlayerChunk.x) + (y-currentPlayerChunk.y)*(y-currentPlayerChunk.y) <=viewRadius*viewRadius)
				{
				
					if (chunkData.chunkRenderer != null) continue;
						
					SpawnChunkRenderer(chunkData);
					if (wait) yield return null;
				}
				else
				{
					if (chunkData.chunkRenderer != null) Destroy(chunkData.chunkRenderer.gameObject);
				}
				
				
			}
		}
		foreach (ChunkData ck in ChunksDatas.Values)
		{
			if ((ck.chunkPosition.x-currentPlayerChunk.x)*(ck.chunkPosition.x-currentPlayerChunk.x) + (ck.chunkPosition.y-currentPlayerChunk.y)*(ck.chunkPosition.y-currentPlayerChunk.y) >viewRadius*viewRadius)
			
			{
				if (ck.chunkRenderer != null) Destroy(ck.chunkRenderer.gameObject);
				ck.State=ChunkDataState.Unloaded;
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
		StartCoroutine(Generate(false));
	}
	ChunkData LoadChunkAt(Vector2Int chunkPosition)
	{
		var xPos= chunkPosition.x*MeshBuilder.chunkWidth*MeshBuilder.blockScale;
		var zPos= chunkPosition.y*MeshBuilder.chunkWidth*MeshBuilder.blockScale;
				
		ChunkData chunkData=new ChunkData();
		chunkData.State= ChunkDataState.StartedLoading;
		chunkData.chunkPosition= chunkPosition;
		
		
		ChunksDatas.Add(chunkPosition, chunkData);
		
		Task.Factory.StartNew(() =>
		{
			chunkData.blocks=terrainGenerator.GenerateTerrain(xPos,zPos);
			chunkData.State= ChunkDataState.Loaded;
		});
				
		return chunkData;
	}	
	void SpawnChunkRenderer(ChunkData chunkData)
	{
		
		ChunksDatas.TryGetValue(chunkData.chunkPosition+Vector2Int.left,out chunkData.leftChunk);
		ChunksDatas.TryGetValue(chunkData.chunkPosition+Vector2Int.right,out chunkData.rightChunk);
		ChunksDatas.TryGetValue(chunkData.chunkPosition+Vector2Int.down,out chunkData.backChunk);
		ChunksDatas.TryGetValue(chunkData.chunkPosition+Vector2Int.up,out chunkData.fwdChunk);
		chunkData.State= ChunkDataState.StartedMeshing;
		
		Task.Factory.StartNew(() =>
		{
			GeneratedMeshData meshData =  MeshBuilder.GeneratedMesh(chunkData);
			
			meshingResult.Enqueue(meshData);
			
		});
	}
	public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
	{
		Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x/MeshBuilder.chunkWidth , blockWorldPos.z/MeshBuilder.chunkWidth);
		
		if(blockWorldPos.x<-MeshBuilder.chunkWidth/2) chunkPosition.x--;
		if(blockWorldPos.z<-MeshBuilder.chunkWidth/2) chunkPosition.y--;
		
		return  chunkPosition;
	}
	
	
	
	
	
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct GeneratedMeshVertex
	{
		public Vector3 pos;
		public sbyte normalX, normalY, normalZ,normalW;
		public ushort uvX,uvY;
	}
	public class GeneratedMeshData
	{
		public GeneratedMeshVertex[] verticies;
		public Bounds bounds;
		public ChunkData data;
	}
}
