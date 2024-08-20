using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
		InfoDataBase.InitBases();
		ChunkRenderer.InitTriangles();
		terrainGenerator.Init();
		StartCoroutine(Generate(false));
	}
	public void PlaceBLock(Dictionary<Vector3Int,BlockType> blocks,bool isDestroy)
	{
		Vector3Int size=PlayerController.Instance.playerState.sizeOfBlockArea;
		Ray  ray = mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f));
			
		if(Physics.Raycast(ray, out var hitInfo,PlayerController.Instance.maxDistanceOfItecation,PlayerController.Instance.playerMovement.groundLayer))
		{
			Vector3 fromBlockToPlayer = new Vector3(PlayerController.Instance.transform.position.x-hitInfo.point.x,0,
													PlayerController.Instance.transform.position.z-hitInfo.point.z).normalized;
			Vector3 downVector;
			Vector3 leftVector;
			float sizeDestroyBlocks;
			float sizeSpawnBlocks;
			int index;
			if(Vector3.Cross(hitInfo.normal,Vector3.up).magnitude==0)
			{
				sizeDestroyBlocks=size.y;
				sizeSpawnBlocks=0.5f;
				if( math.abs(fromBlockToPlayer.x)>=math.abs(fromBlockToPlayer.y))
				{
					downVector=new Vector3(math.sign(fromBlockToPlayer.x)/2,0,0);
					index=3;
				}
				else
				{
					downVector=new Vector3(0,0,math.sign(fromBlockToPlayer.y)/2);
					index=2;
				}
				leftVector= Vector3.Cross(Vector3.up, downVector).normalized/2;
			} 
			else
			{
				downVector=Vector3.down/2;
				if(Vector3.Cross(hitInfo.normal,Vector3.forward).magnitude==0)
				{
					sizeSpawnBlocks=size.z;
					
					index=0;
				}
				else
				{
					sizeSpawnBlocks=size.x;
					index=1;
				}
				sizeDestroyBlocks=0.5f;
				leftVector= Vector3.Cross(Vector3.up, hitInfo.normal).normalized/2;
			}
				
			
			
			Vector3 blockCenter= hitInfo.point/MeshBuilder.blockScale;
			if(size.x>=2&&size.z>=2	&&	size.x%2==0.0f && size.z%2==0.0f)
			{
				if(((hitInfo.point.x/((blockCenter+downVector).x-blockCenter.x))==(hitInfo.point.y/((blockCenter+downVector).y-blockCenter.y)))
				&&((hitInfo.point.x/((blockCenter+downVector).x-blockCenter.x))==(hitInfo.point.z/((blockCenter+downVector).z-blockCenter.z)))
				)
				{
					blockCenter+=downVector*2;
				}
				if(((hitInfo.point.x/((blockCenter+leftVector).x-blockCenter.x))==(hitInfo.point.y/((blockCenter+leftVector).y-blockCenter.y)))
				&&((hitInfo.point.x/((blockCenter+leftVector).x-blockCenter.x))==(hitInfo.point.z/((blockCenter+leftVector).z-blockCenter.z))))
				{
					blockCenter+=leftVector*2;
				}
			}
			blockCenter = blockCenter 
			+ leftVector.normalized* MathF.Ceiling((((Vector3.Cross(leftVector,Vector3.forward).magnitude==0)? size.z:size.x)-2)/2.0f)
			+ downVector.normalized*MathF.Ceiling((((Vector3.Cross(downVector,Vector3.right).magnitude==0)? size.x:size.z) -2)/2.0f);
			
			blockCenter = isDestroy? 
			blockCenter-hitInfo.normal*sizeDestroyBlocks:
			blockCenter+hitInfo.normal*sizeSpawnBlocks;
				
			Vector3Int blockAreaStartPos =Vector3Int.FloorToInt(blockCenter);
			Dictionary<ChunkData,Dictionary<Vector3Int,BlockType>> chunksChanged=new();
			for(int y=0;y<blocks.Last().Key.y+1;y++)
			{
				for(int x=0;x<blocks.Last().Key.x+1;x++)
				{
					for(int z=0;z<blocks.Last().Key.z+1;z++)
					{
						
						Vector3Int blockWorldPos= CalculateBlockPosition(index,x,y,z,leftVector,downVector,hitInfo.normal,blockAreaStartPos);
						
						Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
						//Debug.Log(blockWorldPos.ToString()+" "+blockAreaStartPos.ToString()+" "+chunkPos.ToString());
						
						if(ChunksDatas.TryGetValue(chunkPos, out var chunkData))
						{
							Vector3Int chunkOrigin = new Vector3Int(chunkPos.x*MeshBuilder.chunkWidth,0,chunkPos.y*MeshBuilder.chunkWidth);
							Vector3Int blockChunkPos =blockWorldPos -chunkOrigin;
							//Debug.Log(blockWorldPos.ToString()+" "+chunkOrigin.ToString()+" "+blockChunkPos.ToString());
						
							if(!chunksChanged.ContainsKey(chunkData)) chunksChanged.Add(chunkData,new Dictionary<Vector3Int, BlockType>());
							chunksChanged[chunkData].Add(blockChunkPos,blocks[new Vector3Int(x,y,z)]);
						}
					}
				}
			}
			foreach( var key in chunksChanged.Keys)
			{
				Debug.Log("updade");
				key.chunkRenderer.PlaceBlock(chunksChanged[key]);
			}		
			
		}		
	
	}
	Vector3Int CalculateBlockPosition(int option, int x, int y, int z,Vector3 leftVector,Vector3 downVector, Vector3 normalVector,Vector3Int modifyVector)
	{
		if (vectorCalculations.TryGetValue(option, out Func<int,int, int,Vector3,Vector3,Vector3,Vector3,Vector3Int> newVec))
		{
			return newVec(x, y, z,leftVector,downVector,normalVector,modifyVector);
		}
		else
		{
			Debug.LogWarning($"Option '{option}' not found!");
			return Vector3Int.zero; 
		}
	}
	Dictionary<int, Func<int,int,int,Vector3,Vector3,Vector3,Vector3,Vector3Int>>vectorCalculations = new Dictionary<int, Func<int,int, int,Vector3,Vector3,Vector3,Vector3,Vector3Int>>()
	{
		{0,(x,y,z,leftVector,donwVector,normalVecor,modifyVector)=>Vector3Int.FloorToInt(modifyVector-leftVector*2*x-donwVector*2*y-normalVecor*z) },
		{1,(x,y,z,leftVector,donwVector,normalVecor,modifyVector)=>Vector3Int.FloorToInt(modifyVector-leftVector*2*z-donwVector*2*y-normalVecor*x) },
		{2,(x,y,z,leftVector,donwVector,normalVecor,modifyVector)=>Vector3Int.FloorToInt(modifyVector-leftVector*2*x-donwVector*2*z+normalVecor*y) },
		{3,(x,y,z,leftVector,donwVector,normalVecor,modifyVector)=>Vector3Int.FloorToInt(modifyVector-leftVector*2*z-donwVector*2*x+normalVecor*y) },
				
	};
	void Update()
	{
		Vector3Int playerWorldPos= Vector3Int.FloorToInt(mainCamera.transform.position/MeshBuilder.blockScale);
		Vector2Int playerChunk =GetChunkContainingBlock(playerWorldPos);
		
		if(playerChunk!=currentPlayerChunk	)
		{
			currentPlayerChunk=playerChunk;
			StartCoroutine(Generate(true));
		}
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
		
		if(blockWorldPos.x<0&&!(blockWorldPos.x%32==0)) chunkPosition.x--;
		if(blockWorldPos.z<0&&!(blockWorldPos.z%32==0)) chunkPosition.y--;
		
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
