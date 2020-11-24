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

        float radiansBetweenBranches = 360 / branchesNumber /** 2 * Mathf.PI*/;

        Queue<GameObject> branches = new Queue<GameObject>();

        GameObject root = Instantiate(branch, transform.position, 
            new Quaternion(0, 0, 0, 0), transform);
        root.transform.localScale = new Vector3(thickness, height, thickness);
        branches.Enqueue(root);                

        while (branches.Count != 0)
        {

            GameObject br = branches.Dequeue();

            Vector3 branchScale = br.transform.localScale / 2;

            if (branchScale.y < leafSize / 2)
            {

                Instantiate(leaf,
                    br.transform.localPosition + br.transform.up * branchScale.y,
                    new Quaternion(0, 0, 0, 0),
                    transform).transform.localScale = new Vector3(leafSize, leafSize, leafSize);

                continue;

            }
            
            for (int i = 0; i < branchesNumber; ++i)
            {

                GameObject branchClone = Instantiate(branch,
                    br.transform.localPosition + br.transform.up * branchScale.y,
                    br.transform.localRotation * Quaternion.Euler(-45, radiansBetweenBranches * i, 0),
                    transform);

                branchClone.transform.localScale = branchScale;             

                branches.Enqueue(branchClone);                              

            }

            Instantiate(leaf,
                    br.transform.localPosition + br.transform.up * br.transform.localScale.y,
                    new Quaternion(0, 0, 0, 0),
                    transform).transform.localScale = new Vector3(br.transform.localScale.x * 2, br.transform.localScale.x * 2, br.transform.localScale.x * 2);

        }

    }

}
