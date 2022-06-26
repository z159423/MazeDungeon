using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static void GenerateTerrainMesh(int xSize, int zSize, float scale, int heightMultiply, Vector2 viwerPosition, MeshFilter meshFilter, int levelOfDetail, AnimationCurve _heightCurve, Vector2 offset, bool UseFlatShading)
    {
        //xSize = zSize = CunkSize 중요
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        Vector3[] meshVertices;
        int[] meshTriangles;
        Vector2[] uvs;
        Vector3[] bakedNomals = null;

        bool useFlatShading = UseFlatShading;

        Mesh newMesh;
        newMesh = new Mesh();

        float[,] height = Noise.GenerateNoiseMap(xSize, zSize, scale, viwerPosition + offset);  //노이즈맵 생성

        float topLeftX = (xSize - 1) / -2f;
        float topLeftZ = (zSize - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (xSize - 1) / meshSimplificationIncrement + 1;

        meshVertices = new Vector3[(verticesPerLine) * (verticesPerLine)];
        uvs = new Vector2[meshVertices.Length];
        meshTriangles = new int[((verticesPerLine - 1) * (verticesPerLine - 1) * 6)];

        for (int i = 0, z = 0; z < zSize; z += meshSimplificationIncrement)     // Vertex 생성
        {
            for (int x = 0; x < xSize; x += meshSimplificationIncrement)
            {
                meshVertices[i] = new Vector3(x, heightCurve.Evaluate(height[x, z]) * heightMultiply, z);
                i++;

            }
        }

        int vert = 0;
        int tris = 0;

        for (int i = 0, z = 0; z < zSize; z += meshSimplificationIncrement)   //UV 생성  
        {
            for (int x = 0; x < xSize; x += meshSimplificationIncrement)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }

        for (int z = 0; z < zSize - 1; z += meshSimplificationIncrement)        //Vertex를 기반으로 Triangle 생성
        {
            for (int x = 0; x < xSize - 1; x += meshSimplificationIncrement)
            {
                if (x < xSize - 1 && z < zSize - 1)
                {
                    meshTriangles[tris + 0] = vert;
                    meshTriangles[tris + 1] = vert + verticesPerLine;
                    meshTriangles[tris + 2] = vert + verticesPerLine + 1;
                    meshTriangles[tris + 3] = vert + 1;
                    meshTriangles[tris + 4] = vert;
                    meshTriangles[tris + 5] = vert + verticesPerLine + 1;
                }
                vert++;
                tris += 6;
            }
            vert++;

        }

        Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[meshVertices.Length];
            int triangleCount = meshTriangles.Length / 3;
            for(int i = 0; i < triangleCount; i++)
            {
                int normalTriangleIndex = i * 3;
                int vertexIndexA = meshTriangles[normalTriangleIndex];
                int vertexIndexB = meshTriangles[normalTriangleIndex + 1];
                int vertexIndexC = meshTriangles[normalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            for(int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }
            return vertexNormals;
        }

        Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            Vector3 pointA = meshVertices[indexA];
            Vector3 PointB = meshVertices[indexB];
            Vector3 pointC = meshVertices[indexC];

            Vector3 sideAB = PointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        void Finalize()
        {
            if(useFlatShading)
            {
                FlatShading();
            }
            else
            {
                BakeNormals();
            }
        }

        void BakeNormals()
        {
            bakedNomals = CalculateNormals();
        }

        void FlatShading()
        {
            Vector3[] flatShadedVertices = new Vector3[meshTriangles.Length];
            Vector2[] flatShadedUvs = new Vector2[meshTriangles.Length];

            for (int i = 0; i < meshTriangles.Length; i++)
            {
                flatShadedVertices[i] = meshVertices[meshTriangles[i]];
                flatShadedUvs[i] = uvs[meshTriangles[i]];
                meshTriangles[i] = i;
            }

            meshVertices = flatShadedVertices;
            uvs = flatShadedUvs;
        }
        Finalize();
        newMesh.vertices = meshVertices;
        newMesh.triangles = meshTriangles;
        newMesh.uv = uvs;
        
        if (useFlatShading)
        {
            newMesh.RecalculateNormals();
        }
        else
        {
            newMesh.normals = bakedNomals;
        }
        meshFilter.mesh = newMesh;
    }
}
