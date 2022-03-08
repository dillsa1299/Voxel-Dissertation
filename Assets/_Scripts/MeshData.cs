using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
    }

    public void AddQuadTriangles()
    {
        // Triangle 1 of quad
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        // Triangle 2 of quad
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }
}
