using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{    

    [SerializeField]
    private float height = 5;
    [SerializeField]
    private float thickness = 0.5f;
    [SerializeField]
    private int branchesNumber = 3;
    [SerializeField]
    private float leafSize = 0.3f;

    [SerializeField]
    private GameObject branch = null;
    [SerializeField]
    private GameObject leaf = null;    

    // Start is called before the first frame update
    void Start()
    {        

        GenerateTree();

    }

    // Update is called once per frame
    void Update()
    {

        

    }

    private void GenerateTree()
    {

        CreatePool(out GameObject branchesPool, branch, "Branches Pool");
        CreatePool(out GameObject leafsPool, leaf, "Leafs Pool");

        float radiansBetweenBranches = 360 / branchesNumber;

        Queue<GameObject> branches = new Queue<GameObject>();

        GameObject root = Instantiate(branch, transform.position, 
            new Quaternion(0, 0, 0, 0), branchesPool.transform);
        root.transform.localScale = new Vector3(thickness, height, thickness);
        branches.Enqueue(root);                

        while (branches.Count != 0)
        {

            GameObject parentBranch = branches.Dequeue();

            Vector3 branchScale = parentBranch.transform.localScale / 2;

            if (branchScale.y > leafSize)
            {

                for (int i = 0; i < branchesNumber; ++i)
                {

                    GameObject branchClone = Instantiate(branch,
                        parentBranch.transform.localPosition + parentBranch.transform.up * branchScale.y,
                        parentBranch.transform.localRotation * Quaternion.Euler(-45, radiansBetweenBranches * i, 0),
                        branchesPool.transform);

                    branchClone.transform.localScale = branchScale;

                    branches.Enqueue(branchClone);

                }

            }

            Instantiate(leaf,
                    parentBranch.transform.localPosition + parentBranch.transform.up * parentBranch.transform.localScale.y,
                    parentBranch.transform.localRotation/*new Quaternion(0, 0, 0, 0)*/,
                    leafsPool.transform).transform.localScale
                    = new Vector3(parentBranch.transform.localScale.x * 2,
                    parentBranch.transform.localScale.x * 2,
                    parentBranch.transform.localScale.x * 2);

        }

        CombineMesh(branchesPool);
        CombineMesh(leafsPool);

    }

    private void CreatePool(out GameObject pool, GameObject type, string name)
    {

        pool = new GameObject(name, typeof(MeshFilter));

        pool.transform.parent = transform;

        MeshRenderer bpMeshRenderer = pool.AddComponent<MeshRenderer>();
        bpMeshRenderer.material = type.GetComponentInChildren<MeshRenderer>().sharedMaterial;

    }

    private void CombineMesh(GameObject obj)
    {

        //Set position to zero for easier matrix math
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];        

        //From 1 cause 0 is parent
        for (int i = 1; i < meshFilters.Length; ++i)
        {

            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);

        }

        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        //Return to initial position
        obj.transform.position = position;

    }

}
