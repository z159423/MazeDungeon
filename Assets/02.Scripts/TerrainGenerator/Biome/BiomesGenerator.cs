using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;
using Delaunay.Geo;

public class BiomesGenerator : MonoBehaviour
{

    public int MaxMap_width = 100000;
    public int MaxMap_height = 100000;
    public int BiomesPointCount = 300;
    public List<Vector2> V_Point = new List<Vector2>();
    private List<LineSegment> m_edges = null;
    public float SphereRadius = 10f;



    void Start()
    {
        GenerateBiomesPoint();
    }

    void Update()
    {

    }

    private void GenerateBiomesPoint()
    {
        List<uint> colors = new List<uint>();

        for (int i = 0; i < BiomesPointCount; i++)
        {
            colors.Add(0);
            V_Point.Add(new Vector2(
                Random.Range(-MaxMap_width, MaxMap_width),
                Random.Range(-MaxMap_height, MaxMap_height)
                ));
        }

        Delaunay.Voronoi v = new Delaunay.Voronoi(V_Point, colors, new Rect(0, 0 , MaxMap_width, MaxMap_height));
        m_edges = v.VoronoiDiagram();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (V_Point != null)
        {
            for (int i = 0; i < V_Point.Count; i++)
            {
                Gizmos.DrawSphere(new Vector3(V_Point[i].x,0,V_Point[i].y), SphereRadius);
            }
        }

        if (m_edges != null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector2 left = (Vector2)m_edges[i].p0;
                Vector3 point1 = new Vector3(m_edges[i].p0.Value.x, 0, m_edges[i].p0.Value.y);
                Gizmos.DrawSphere(point1, SphereRadius);

                Vector2 right = (Vector2)m_edges[i].p1;
                Vector3 point2 = new Vector3(m_edges[i].p1.Value.x, 0, m_edges[i].p1.Value.y);
                Gizmos.DrawSphere(point2, SphereRadius);

                Gizmos.DrawLine(point1, point2);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-MaxMap_width, 0, -MaxMap_height), new Vector3(-MaxMap_width, 0 ,MaxMap_height));
        Gizmos.DrawLine(new Vector3(-MaxMap_width, 0, -MaxMap_height), new Vector3(MaxMap_width,0, -MaxMap_height));
        Gizmos.DrawLine(new Vector3(MaxMap_width,0, -MaxMap_height), new Vector3(MaxMap_width,0, MaxMap_height));
        Gizmos.DrawLine(new Vector3(-MaxMap_width, 0, MaxMap_height), new Vector3(MaxMap_width,0, MaxMap_height));

    }

}
