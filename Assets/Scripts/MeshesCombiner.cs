using System.Collections.Generic;
using UnityEngine;

public class MeshesCombiner
{
    public static void CombineMeshes(Transform pool, GameObject type)
    {
        int meshVertexesCount = 0;
        int meshHandlerIndex = 0;

        List<CombineInstance> combine = new List<CombineInstance>();

        MeshFilter[] meshFilters = pool.GetComponentsInChildren<MeshFilter>();

        CreateSubMesh(pool, type, meshHandlerIndex, out GameObject meshHandler, out Vector3 position);

        foreach (var mesh in meshFilters)
        {
            if (mesh.sharedMesh.vertexCount + meshVertexesCount > 65535)
            {
                Combine(meshHandler, position, combine.ToArray());

                combine.Clear();

                ++meshHandlerIndex;
                meshVertexesCount = mesh.sharedMesh.vertexCount;

                CreateSubMesh(pool, type, meshHandlerIndex, out meshHandler, out position);
            }
            else
            {
                meshVertexesCount += mesh.sharedMesh.vertexCount;
            }

            CombineInstance ci = new CombineInstance();

            ci.mesh = mesh.sharedMesh;
            ci.transform = mesh.transform.localToWorldMatrix;

            combine.Add(ci);

#if UNITY_EDITOR

            UnityEngine.Object.DestroyImmediate(mesh.gameObject.transform.parent.gameObject);

#else

            Object.Destroy(meshFilters[i].gameObject.transform.parent.gameObject);

#endif

        }

        Combine(meshHandler, position, combine.ToArray());
    }

    private static void Combine(GameObject meshHandler, Vector3 position, CombineInstance[] combine)
    {
        meshHandler.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        meshHandler.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
        meshHandler.transform.gameObject.SetActive(true);

        //NOTE: Return to initial position
        meshHandler.transform.position = position;
    }

    private static void CreateSubMesh(Transform parent, GameObject type, int index, out GameObject MeshHandler, out Vector3 position)
    {
        MeshHandler = new GameObject("MeshHandler" + index.ToString(), typeof(MeshFilter));

        MeshHandler.transform.parent = parent;

        MeshRenderer bpMeshRenderer = MeshHandler.AddComponent<MeshRenderer>();
        bpMeshRenderer.material = type.GetComponentInChildren<MeshRenderer>().sharedMaterial;

        //NOTE: Set position to zero for easier matrix math
        position = MeshHandler.transform.position;
        MeshHandler.transform.position = Vector3.zero;
    }
}
