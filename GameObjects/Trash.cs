using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

public class Trash : MonoBehaviour
{
    public int Worth;
    private TrashNode ParentNode;
    private Randomizer rnd;
    private AudioSource AS;
    private bool Active;

    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    void Awake()
    {
        rnd = Randomizer.getInstance();
        Active = true;
    }
    public void SetParent(TrashNode Node)
    {
        ParentNode = Node;
    }

    public void Recycle()
    {
        if (Active)
        {
            Active = false;
            AS.Play();
            ParentNode.RemoveTrash(this);
            LODGroup tempLOD = GetComponent<LODGroup>();
            if (tempLOD != null)
            {
                int children = transform.childCount;
                for (int i = 0; i < children; ++i)
                    transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
            Destroy(gameObject,1.2f);
        }
    }

    

    public void RandomizeLook()
    {
        RandomizeLocation();
        RandomizeOrientation();
    }
    public void RandomizeLocation()
    {
        transform.localPosition = Vector3.zero;
        System.Random rnd = new Random();
        transform.localPosition = new Vector3(2.5f - rnd.Next(50)/ 10, 0, 2.5f- rnd.Next(50) / 10) ;
    }

    public void RandomizeOrientation()
    {
        if(rnd.getRandom(6) == 5)
            transform.rotation = Quaternion.Euler(270, 0,0);
        else
        {
            transform.rotation = Quaternion.Euler(-0, rnd.getRandom(270), 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
