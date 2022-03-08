using System.Collections;
using System.Collections.Generic;
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
    }

    public void InitializeChunk(ChunkData data)
    {
        this.ChunkData = data;
    }

    private void RenderMesh(MeshData meshData)
    {
        mesh.Clear(); // Reset mesh

        mesh.vertices = meshData.vertices.ToArray(); // Set vertices
        mesh.SetTriangles(meshData.triangles.ToArray(), 0); // Set triangles

        mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh; // Sets collision mesh same as render mesh
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
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
