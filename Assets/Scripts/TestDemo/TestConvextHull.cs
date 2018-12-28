using KayAlgorithm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KayMath;


public class TestConvextHull : MonoBehaviour
{
    List<Vector3> origins = new List<Vector3>();

    private void Awake()
    {
        var skin = gameObject.GetComponent<SkinnedMeshRenderer>();
        var mesh = skin.sharedMesh;
        int subCnt = mesh.subMeshCount;
        origins.AddRange(mesh.vertices);
        int[] indices = mesh.triangles;
        GameObject res = new GameObject("Origin");
        var filter = res.AddComponent<MeshFilter>();
        var render = res.AddComponent<MeshRenderer>();
        Mesh mesNewh = new Mesh();
        mesNewh.SetVertices(origins);
        mesNewh.SetTriangles(indices, 0);
        filter.mesh = mesNewh;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Jarvis March", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleJarvisHull();
        }

        if (GUILayout.Button("Graham's Scan", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleGrahamScan();
        }

        if (GUILayout.Button("Andrew's Monotone Chain", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleMonotoneChain();
        }

        if (GUILayout.Button("Chan's Convex", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleChanConvex();
        }

        if (GUILayout.Button("Quick Hull", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleQuickHull();
        }

        if (GUILayout.Button("Incremental Convex", GUILayout.Width(500), GUILayout.Height(60)))
        {
            HandleIncremental();
        }
        

    }

    private void HandleJarvisHull()
    {
        List<Vector2> xy = new List<Vector2>();
        foreach (var item in origins)
        {
            xy.Add(item);
        }
        List<Vector2> vertices = JarvisConvex.BuildHull(xy, 1);
        DrawPolygon(vertices, Color.red);
    }

    private void HandleGrahamScan()
    {
        List<Vector2> xy = new List<Vector2>();
        foreach (var item in origins)
        {
            xy.Add(item);
        }
        List<Vector2> res = GrahamScanConvex.BuildConvex(xy);
        DrawPolygon(res, Color.blue);
    }

    private void HandleMonotoneChain()
    {
        List<Vector2> xy = new List<Vector2>();
        foreach (var item in origins)
        {
            xy.Add(item);
        }
        List<Vector2> res = MonotoneChainConvex.BuildConvex(xy);
        DrawPolygon(res, Color.yellow);
    }
    private void DrawPolygon(List<Vector2> vertices, Color clr)
    {
        for (int i = 0; i < vertices.Count; ++i)
        {
            int j = i + 1;
            if (j == vertices.Count)
            {
                j = 0;
            }
            Debug.DrawLine(vertices[i], vertices[j], clr, 10000000000);
        }
    }

    private void HandleQuickHull()
    {
        var results = QuickHull.BuildHull(origins);
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        GeoUtils.MeshVertexPrimitiveType(results, ref vertices, ref indices);
        GameObject res = new GameObject("QuickHull");
        var filter = res.AddComponent<MeshFilter>();
        var render = res.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(indices, 0);
        filter.mesh = mesh;
    }

    private void HandleIncremental()
    {
        List<Vector2> xy = new List<Vector2>();
        foreach (var item in origins)
        {
            xy.Add(item);
        }
        List<Vector2> res = IncrementalConvex.BuildConvex(xy);
        DrawPolygon(res, Color.green);
    }

    private void HandleChanConvex()
    {
        List<Vector2> xy = new List<Vector2>();
        foreach (var item in origins)
        {
            xy.Add(item);
        }
        List<Vector2> res = IncrementalConvex.BuildConvex(xy);
        DrawPolygon(res, Color.cyan);
    }
}

