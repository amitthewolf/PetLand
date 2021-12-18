using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class EventManager : MonoBehaviour
{
    public static EventHandler TrashEmptied;
    public static EventHandler PetLookingForTrash;
    public static EventHandler PetReachedTrashNode;
    public static EventHandler PetReachedWanderDestination;
    public static EventHandler PetSpawned;
    public static EventHandler CallFullSave;

    public TrashMaster TM;
    private float LastSaveTime;
    public bool Load;

    // Start is called before the first frame update
    void Start()
    {
        CallFullSave += Save;
    }
    
    private void OnDestroy()
    {
        CallFullSave -= Save;
    }

    public void Save(Object sender, EventArgs args)
    {
        SavingSystem.SaveProgress(Player.Animals, TM.GetTrashAmount(), DateTime.Now);
    }
    
    void Awake()
    {
        if (Load)
        {
            try
            {
                SaveData data = SavingSystem.LoadProgress();
                if (data == null)
                {
                    print("Changing Tutorial");
                    Player.Tutorial = true;
                    Player.Budget = 5;
                    return;
                }
                if(Player.Tutorial && Player.Budget != 0)
                    Player.Tutorial = false;
                Player.Animals = data.Animals;
                Player.Budget = PlayerPrefs.GetInt("Budget");
                TM.SetTrashAmountOnLoad(PlayerPrefs.GetInt("TrashAmount"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            Player.Budget = 5;
        }
    }

}
