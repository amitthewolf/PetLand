using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PetSpawner : MonoBehaviour
{
    public List<GameObject> PetPrefabs;
    private List<GameObject> PetsInPlay;
    private int CurrentIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        PetsInPlay = new List<GameObject>();
        if(Player.Animals.Count > 0)
            Player.Animals.ForEach(Pet =>SpawnPet(Pet));
    }
    private void SpawnPet(Animal Pet)
    {
        print("SpawningPet");
        GameObject NewPet = null;
        switch (Pet.getSpecies())
        {
            case Animal.Species.Llama:
                NewPet = Instantiate(PetPrefabs[0],gameObject.transform);
                NewPet.GetComponent<PetBrain>().setAnimal(Pet);
                NewPet.GetComponent<NavMeshAgent>().Warp(RandomNavSphere(transform.position, 15f, -1));
                break;
            case Animal.Species.Duck:
                NewPet = Instantiate(PetPrefabs[1],gameObject.transform);
                NewPet.GetComponent<PetBrain>().setAnimal(Pet);
                NewPet.GetComponent<NavMeshAgent>().Warp(RandomNavSphere(transform.position, 15f, -1));
                break;
            case Animal.Species.Rabbit:
                NewPet = Instantiate(PetPrefabs[2],gameObject.transform);
                NewPet.GetComponent<PetBrain>().setAnimal(Pet);
                NewPet.GetComponent<NavMeshAgent>().Warp(RandomNavSphere(transform.position, 15f, -1));
                break;
            case Animal.Species.Cat:
                NewPet = Instantiate(PetPrefabs[2],gameObject.transform);
                NewPet.GetComponent<PetBrain>().setAnimal(Pet);
                NewPet.GetComponent<NavMeshAgent>().Warp(RandomNavSphere(transform.position, 15f, -1));
                break;
        }
        if (NewPet)
        {
            PetsInPlay.Add(NewPet);
            EventManager.PetSpawned(NewPet, EventArgs.Empty);
            CurrentIndex = PetsInPlay.Count;
        }
    }
    
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        Vector3 ToReturn = new Vector3(
            Mathf.Clamp(navHit.position.x, -18f, 19f),
            navHit.position.y,
            Mathf.Clamp(navHit.position.z, -21f, 17));
        return ToReturn;
    }
    
    public void NextPet()
    {
        if (CurrentIndex >= PetsInPlay.Count-1)
            CurrentIndex = 0;
        else
            CurrentIndex++;
        CameraController.TargetPet = PetsInPlay[CurrentIndex];
    }
    
    public void PrevPet()
    {
        if (CurrentIndex == 0)
            CurrentIndex = PetsInPlay.Count-1;
        else
            CurrentIndex--;
        CameraController.TargetPet = PetsInPlay[CurrentIndex];
    }
    
    private void OnEnable()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            NextPet();
    }
}
