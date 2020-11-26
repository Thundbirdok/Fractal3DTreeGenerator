using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshesCombiner
{

    public static void CombineMeshes(GameObject obj)
    {

        //Set position to zero for easier matrix math
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

        if (Application.IsPlaying(obj))
        {

            Debug.Log("Combine in Game");

        }
        else
        {

            Debug.Log("Combine in Editor");

        }

        //From 1 cause 0 is parent
        for (int i = 1; i < meshFilters.Length; ++i)
        {

            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;

            if (Application.IsPlaying(obj))
            {

                Object.Destroy(meshFilters[i].gameObject);                

            }
            else
            {

#if UNITY_EDITOR

                Object.DestroyImmediate(meshFilters[i].gameObject);

#endif

            }            

        }

        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        //Return to initial position
        obj.transform.position = position;

    }

}
