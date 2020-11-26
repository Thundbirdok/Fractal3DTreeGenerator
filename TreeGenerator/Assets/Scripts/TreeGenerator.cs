using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    [Header("Branch Settings")]

    [SerializeField]
    private float height = 5;
    [SerializeField]
    private float thickness = 0.5f;
    [SerializeField]
    private int branchesNumber = 3;
    [SerializeField]
    private float branchAngle = 45;
    [SerializeField]
    private float branchScaler = 0.5f;

    [Header("Leaf Settings")]

    [SerializeField]
    private bool rotateLeafs = false;
    [SerializeField]
    private float leafSize = 0.3f;    
    [SerializeField]
    private float leafScaler = 2f;

    [Header("Materilals")]

    [SerializeField]
    private GameObject branch = null;
    [SerializeField]
    private GameObject leaf = null;

    private GameObject branchesPool;
    private GameObject leafsPool;

    private float degreesBetweenBranches;

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

        degreesBetweenBranches = 360 / branchesNumber;

        CreatePool(out branchesPool, branch, "Branches Pool");
        CreatePool(out leafsPool, leaf, "Leafs Pool");        

        Queue<GameObject> branches = new Queue<GameObject>();        
        
        branches.Enqueue(CreateRoot());                

        while (branches.Count != 0)
        {

            GameObject parentBranch = branches.Dequeue();

            Vector3 branchScale = parentBranch.transform.localScale * branchScaler;

            if (branchScale.y > leafSize)
            {
                       
                foreach (var obj in CreateBranch(parentBranch, branchScale))
                {

                    branches.Enqueue(obj);

                }                

            }

            CreateLeaf(parentBranch);            

        }

        MeshesCombiner.CombineMeshes(branchesPool);        
        MeshesCombiner.CombineMeshes(leafsPool);

    }

    private GameObject CreateRoot()
    {

        GameObject root = Instantiate(branch, transform.position,
            new Quaternion(0, 0, 0, 0), branchesPool.transform);

        root.transform.localScale = new Vector3(thickness, height, thickness);

        return root;

    }

    private IEnumerable<GameObject> CreateBranch(GameObject parentBranch, Vector3 branchScale)
    {

        for (int i = 0; i < branchesNumber; ++i)
        {

            Vector3 branchPosition = parentBranch.transform.localPosition 
                + parentBranch.transform.up * branchScale.y;

            Quaternion leafRotation = parentBranch.transform.localRotation 
                * Quaternion.Euler(branchAngle, degreesBetweenBranches * i, 0);           

            GameObject branchClone = Instantiate(branch,
                        branchPosition, leafRotation,
                        branchesPool.transform);

            branchClone.transform.localScale = branchScale;

            yield return branchClone;

        }

    }

    private void CreateLeaf(GameObject parentBranch)
    {

        Vector3 leafPosition = parentBranch.transform.localPosition
                + parentBranch.transform.up * parentBranch.transform.localScale.y;

        Quaternion leafRotation;

        if (rotateLeafs)
        {

            leafRotation = parentBranch.transform.localRotation;

        }
        else
        {

            leafRotation = new Quaternion(0, 0, 0, 0);

        }

        GameObject leafClone = Instantiate(leaf, leafPosition, leafRotation,
            leafsPool.transform);

        float leafScale = parentBranch.transform.localScale.x * leafScaler;

        leafClone.transform.localScale = new Vector3(leafScale, leafScale, leafScale);

    }

    private void CreatePool(out GameObject pool, GameObject type, string name)
    {

        pool = new GameObject(name, typeof(MeshFilter));

        pool.transform.parent = transform;

        MeshRenderer bpMeshRenderer = pool.AddComponent<MeshRenderer>();
        bpMeshRenderer.material = type.GetComponentInChildren<MeshRenderer>().sharedMaterial;

    }    

}
