using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int Budget;
    public static List<Animal> Animals;
    public static bool Tutorial;

    void Awake()
    {
        EventManager.TrashEmptied += AddBudget;
        if(Animals == null)
            Animals = new List<Animal>();
    }

    // private void AddPet(object sender, EventArgs e)
    // {
    //     Animals.Add((Animal)sender);
    //     EventManager.CallAnimalSave(this,EventArgs.Empty);
    // }

    private void OnDestroy()
    {
        EventManager.TrashEmptied -= AddBudget;
    }
    
    public void setAnimals(List<Animal> animals)
    {
        Animals = animals;
    }
    
    public void AddBudget(object sender, EventArgs args)
    {
        Budget += ((TrashNode)sender).GetWorth() + 1;
        PlayerPrefs.SetInt("Budget",Budget);
        PlayerPrefs.Save();
    }
    
    public static void RemoveBudget(int ToRemove)
    {
        Budget -= ToRemove;
        PlayerPrefs.SetInt("Budget",Budget);
        PlayerPrefs.Save();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
