using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Cubesupport : MonoBehaviour
{
    public int polygon = 3;
    public float size = 1.0f;
    public float height = 1.0f;
    public Vector3 offset = new Vector3(0, 0, 0);

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    void OnValidate()
    {
        if (mesh == null) return;

        if (size > 0 || offset.magnitude > 0 || polygon >= 3 || height > 0)
        {
            setMeshData(size, polygon);
            createProceduralMesh();
        }
    }

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        setMeshData(size, polygon);
        createProceduralMesh();
    }

    void setMeshData(float size, int polygon)
    {
        vertices = new Vector3[polygon + 1 + (3 * polygon)];

        vertices[0] = new Vector3(0, -height / 2.0f, 0) + offset;
        for (int i = 1; i <= polygon; i++)
        {
            float angle = -i * (Mathf.PI * 2.0f) / polygon;

            vertices[i]
                = (new Vector3(Mathf.Cos(angle) * size, -height / 2.0f, Mathf.Sin(angle) * size)) + offset;
        }

        triangles = new int[3 * polygon + 3 * polygon];
        for (int i = 0; i < polygon - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        triangles[3 * polygon - 3] = 0;
        triangles[3 * polygon - 2] = 1;
        triangles[3 * polygon - 1] = polygon;

        /* -------------------------------------------------------- */

        int tIdx = 3 * polygon;
        Vector3 point = new Vector3(0, height / 2.0f, 0) + offset;
        for (int i = 1; i <= polygon - 1; i++)
        {
            vertices[polygon + 1 + 3 * i - 3] = point;
            vertices[polygon + 1 + 3 * i - 2] = vertices[i];
            vertices[polygon + 1 + 3 * i - 1] = vertices[i + 1];

            triangles[tIdx++] = polygon + 1 + 3 * i - 3;
            triangles[tIdx++] = polygon + 1 + 3 * i - 2;
            triangles[tIdx++] = polygon + 1 + 3 * i - 1;
        }

        vertices[polygon + 1 + (3 * polygon) - 1 - 2] = point;
        vertices[polygon + 1 + (3 * polygon) - 1 - 1] = vertices[polygon];
        vertices[polygon + 1 + (3 * polygon) - 1 - 0] = vertices[1];

        triangles[tIdx++] = polygon + 1 + (3 * polygon) - 1 - 2;
        triangles[tIdx++] = polygon + 1 + (3 * polygon) - 1 - 1;
        triangles[tIdx++] = polygon + 1 + (3 * polygon) - 1 - 0;
    }

    void createProceduralMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Destroy(this.GetComponent<MeshCollider>());
        this.gameObject.AddComponent<MeshCollider>();
    }
}
