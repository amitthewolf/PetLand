using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashNode : MonoBehaviour
{
    public List<GameObject> Trash;
    private int Worth;
    private int counter;
    private bool Active;
    private GameObject IncomingPet;

    public int GetWorth()
    {
        int toReturn = Worth;
        Worth = 0;
        return toReturn;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PetIncoming(GameObject SeekingPet)
    {
        Active = true;
        IncomingPet = SeekingPet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Active && other.gameObject.CompareTag("Pet"))
        {
            if (IncomingPet == other.gameObject)
            {
                EventManager.PetReachedTrashNode(other.gameObject, EventArgs.Empty);
                Active = false;
            }
        }
    }


    public void AddTrash()
    {
        int index = Randomizer.getInstance().getRandom(Trash.Count);
        GameObject NewTrash = Instantiate(Trash[index], this.gameObject.transform);
        Trash trash = NewTrash.GetComponent<Trash>();
        trash.SetParent(this);
        trash.RandomizeLook();
        Worth += trash.Worth;
        counter++;
    }

    public int getTrashAmount()
    {
        return counter;
    }

    public void RemoveTrash(Trash RecycledTrash)
    {
        counter--;
        if (counter == 0)
        {
            //EventManager.TrashEmptiedArgs args = new EventManager.TrashEmptiedArgs();
            //args.worth = Worth;
            EventManager.TrashEmptied(this,EventArgs.Empty);
        }
    }

    // Update is called once per frame
    
}
