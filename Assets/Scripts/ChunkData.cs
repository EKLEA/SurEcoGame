

using System.Numerics;
using UnityEngine;

public class ChunkData 
{
	public ChunkDataState State;
	public Vector2Int chunkPosition;
	public ChunkRenderer chunkRenderer;
	public BlockType[] blocks;
	public ChunkData leftChunk;
	public ChunkData rightChunk;
	public ChunkData fwdChunk;
	public ChunkData backChunk;
}
public enum ChunkDataState
{
	StartedLoading,
	Loaded,
	StartedMeshing,
	SpawnedInWorld,
	Unloaded
}
