using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TreeGenerator : MonoBehaviour
{

    public enum GenerationType
    {

        Cross,
        Y

    }

    [Header("Branch Settings")]    

    [SerializeField]
    private GenerationType type = GenerationType.Cross;

    [SerializeField]
    private float height = 5;
    [SerializeField]
    private float thickness = 0.5f;
    [SerializeField]
    private uint branchNumberOnLevel = 3;
    [SerializeField]
    private uint levelNumber = 2;
    [SerializeField]
    private float branchAngle = 45;
    [SerializeField, Range(0.1f, 0.9f)]
    private float branchScaler = 0.5f;

    [Header("Leaf Settings")]

    [SerializeField]
    private bool rotateLeafs = false;
    [SerializeField]
    private float endSize = 0.3f;
    [SerializeField]
    private float leafScaler = 2f;

    [Header("Materilals")]

    [SerializeField]
    private GameObject branch = null;
    [SerializeField]
    private GameObject leaf = null;

    [SerializeField, HideInInspector]
    private GameObject branchesPool;
    [SerializeField, HideInInspector]
    private GameObject leafsPool;    

    private float degreesBetweenBranches;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTree()
    {

        degreesBetweenBranches = 360 / branchNumberOnLevel;

        CreatePool(ref branchesPool, branch, "Branches Pool");
        CreatePool(ref leafsPool, leaf, "Leafs Pool");

        Queue<GameObject> branches = new Queue<GameObject>();

        branches.Enqueue(CreateRoot());

        while (branches.Count != 0)
        {

            GameObject parentBranch = branches.Dequeue();

            Vector3 branchScale = parentBranch.transform.localScale * branchScaler;

            if (branchScale.y > endSize)
            {

                foreach (var obj in CreateBranch(parentBranch, branchScale))
                {

                    branches.Enqueue(obj);

                }

            }
            else if (type == GenerationType.Y)
            {

                CreateLeaf(parentBranch);

                continue;

            }

            if (type == GenerationType.Cross)
            {
             
                CreateLeaf(parentBranch);

            }

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

        for (int i = 1; i <= levelNumber; ++i)
        {

            for (int j = 0; j < branchNumberOnLevel; ++j)
            {

                Vector3 branchPosition;

                switch (type)
                {

                    case GenerationType.Cross:

                        branchPosition = parentBranch.transform.localPosition
                        + parentBranch.transform.up * parentBranch.transform.localScale.y * ((float)i / (levelNumber + 1));

                        break;

                    case GenerationType.Y:

                        branchPosition = parentBranch.transform.localPosition
                        + parentBranch.transform.up * parentBranch.transform.localScale.y * ((float)i / levelNumber);

                        break;

                    default:

                        branchPosition = Vector3.zero;

                        break;

                }

                Quaternion leafRotation = parentBranch.transform.localRotation
                    * Quaternion.Euler(branchAngle, degreesBetweenBranches * j, 0);

                GameObject branchClone = Instantiate(branch,
                            branchPosition, leafRotation,
                            branchesPool.transform);

                branchClone.transform.localScale = branchScale / i;
                branchClone.transform.position += branchClone.transform.forward * branchScale.x / 2;

                yield return branchClone;

            }

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

    private void CreatePool(ref GameObject pool, GameObject type, string name)
    {

#if UNITY_EDITOR

        DestroyImmediate(pool);

#else
               
            Destroy(pool);            

#endif

        pool = new GameObject(name, typeof(MeshFilter));

        pool.transform.parent = transform;

        MeshRenderer bpMeshRenderer = pool.AddComponent<MeshRenderer>();
        bpMeshRenderer.material = type.GetComponentInChildren<MeshRenderer>().sharedMaterial;

    }

}
