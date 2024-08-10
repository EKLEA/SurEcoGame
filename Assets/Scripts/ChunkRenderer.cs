using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer),typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
	
	public GameWorld parentWorld;
	
	Mesh chunkMesh;
	
	
	
	public ChunkData chunkData;
	
	static int[] triangles;
	public static void InitTriangles()
	{
		triangles=new int[65536*6/4];
		int vertexsNum=4;
		for (int i=0; i<triangles.Length;i+=6)
		{
			triangles[i]=vertexsNum-4;
			triangles[i+1]=vertexsNum-3;
			triangles[i+2]=vertexsNum-2;


			triangles[i+3]=vertexsNum-3;
			triangles[i+4]=vertexsNum-1;
			triangles[i+5]=vertexsNum-2;
			vertexsNum+=4;
		}
	}


	private void Awake() 
	{
		
		chunkMesh = new Mesh();

		GetComponent<MeshFilter>().mesh = chunkMesh;
	}
	public void SetMesh(GameWorld.GeneratedMeshData meshData)
	{
		var layout = new[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position,VertexAttributeFormat.Float32,3),
			new VertexAttributeDescriptor(VertexAttribute.Normal,VertexAttributeFormat.SNorm8,4),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0,VertexAttributeFormat.UNorm16,2),
		};
		chunkMesh.SetVertexBufferParams(meshData.verticies.Length,layout);
		chunkMesh.SetVertexBufferData(meshData.verticies,0,0,meshData.verticies.Length,0,
		MeshUpdateFlags.DontRecalculateBounds|MeshUpdateFlags.DontValidateIndices|MeshUpdateFlags.DontResetBoneBounds);
		
		int trianglesCount=meshData.verticies.Length/4*6;

		chunkMesh.SetIndexBufferParams(trianglesCount,IndexFormat.UInt32);
		chunkMesh.SetIndexBufferData(triangles,0,0,trianglesCount,
		MeshUpdateFlags.DontRecalculateBounds|MeshUpdateFlags.DontValidateIndices|MeshUpdateFlags.DontResetBoneBounds);
		
		chunkMesh.subMeshCount=1;
		chunkMesh.SetSubMesh(0,new SubMeshDescriptor(0,trianglesCount));
		
		chunkMesh.bounds= meshData.bounds;
		
		GetComponent<MeshCollider>().sharedMesh = chunkMesh;
	}
	public void SpawnBlock(Vector3Int blockPosition)
	{
		int index =blockPosition.x + blockPosition.y * MeshBuilder.chunkWidthSq + blockPosition.z*MeshBuilder.chunkWidth;
		chunkData.blocks[index] = BlockType.Stone;
		RegenerateMesh();
	}
	
	public void DestroyBlock(Vector3Int blockPosition)
	{
		int index =blockPosition.x + blockPosition.y * MeshBuilder.chunkWidthSq + blockPosition.z*MeshBuilder.chunkWidth;
		chunkData.blocks[index] = BlockType.Air;
		RegenerateMesh();
	}
	void RegenerateMesh()
	{
		SetMesh(MeshBuilder.GeneratedMesh(chunkData));
	}
	

}
