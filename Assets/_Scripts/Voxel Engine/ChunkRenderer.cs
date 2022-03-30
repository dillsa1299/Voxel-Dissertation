using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Automatically adds components
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;
    public bool showGizmo = false;

    private bool inRenderDistance = false;

    private bool chunkIsRendered = false;

    public ChunkData ChunkData { get; private set;}

    public bool modified
    {
        get
        {
            return ChunkData.modified;
        }
        set
        {
            ChunkData.modified = value;
        }
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;

        //Increases mesh vertices limit from ~65k to ~4mil
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        //Without doing so large chunks will not render fully
        
    }

    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData)
    {
        if (inRenderDistance)
        {
            if (!chunkIsRendered)
            {
                mesh.Clear(); // Reset mesh
                mesh.subMeshCount = 2; //Main mesh & water mesh
                mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray(); // Set vertices
                
                mesh.SetTriangles(meshData.triangles.ToArray(), 0); // Set main mesh triangles
                mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);

                mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
                mesh.RecalculateNormals();

                meshCollider.sharedMesh = null; 
                Mesh collisionMesh = new Mesh();
                collisionMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                collisionMesh.vertices = meshData.colliderVertices.ToArray();
                collisionMesh.triangles = meshData.colliderTriangles.ToArray();
                collisionMesh.RecalculateNormals();

                meshCollider.sharedMesh = collisionMesh; // Sets collision mesh same as render mesh
                chunkIsRendered = true;
            }
        }
        if (chunkIsRendered && !inRenderDistance)
        {
            mesh.Clear(); // Reset mesh
            chunkIsRendered = false;
        }
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

    public void UpdateChunk(Vector3 playerPos, int renderDistance)
    {
        //var temp = Time.realtimeSinceStartup;
        Vector3 centerPos = ChunkData.worldPos;
        centerPos.x = centerPos.x + (ChunkData.size/2);
        centerPos.z = centerPos.z + (ChunkData.size/2);
        if(Vector3.Distance(ChunkData.worldPos, playerPos) < renderDistance)
        {
            inRenderDistance = true;
        }
        else
        {
            inRenderDistance = false;
        }
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
        //print("Time for RenderMesh: " + (Time.realtimeSinceStartup - temp).ToString("f6"));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() //Shows volume of selected chunk in Unity Editor
    {
        
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                Gizmos.DrawCube(transform.position + new Vector3(ChunkData.size / 2f, ChunkData.height / 2f,
                    ChunkData.size / 2f), new Vector3(ChunkData.size, ChunkData.height, ChunkData.size));
            }
        }
    }
#endif
}
