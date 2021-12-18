using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    private GameObject IncomingPet;

    public void SetIncomingPet(GameObject Pet)
    {
        IncomingPet = Pet;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pet"))
        {
            if (IncomingPet == other.gameObject)
            {
                EventManager.PetReachedWanderDestination(other.gameObject, EventArgs.Empty);
                Destroy(this.gameObject);
            }
        }
    }
}
