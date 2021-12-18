using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class TrashMaster : MonoBehaviour
{
    [FormerlySerializedAs("TrashNodes")] public List<TrashNode> availableTrashNodes;
    public float TrashSpawnRate;
    public int MaxCapacity;
    public static float TrashCapacity;
    private Hashtable TrashCounts;
    private float LastSpawn;
    private Randomizer rnd;
    private DateTime TempDate;

    private int TrashNodeNum = 4;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (TrashCounts == null)
            TrashCounts = new Hashtable();
        EventManager.TrashEmptied += EmptyNode;
        EventManager.PetLookingForTrash += SeekingTrash;
        TempDate = DateTime.MinValue;
    }
    
    private void OnDestroy()
    {
        EventManager.TrashEmptied -= EmptyNode;
        EventManager.PetLookingForTrash -= SeekingTrash;
    }
    private void EmptyNode(object sender, EventArgs args)
    {
        TrashCounts.Remove(sender);
        if(!availableTrashNodes.Contains((TrashNode)sender))
            availableTrashNodes.Add((TrashNode)sender);
        SavingSystem.SaveProgress(Player.Animals, GetTrashAmount(), DateTime.Now);
    }

    private void SeekingTrash(object sender, EventArgs args)
    {
        GameObject SeekingPet = ((PetBrain) sender).gameObject;
        float dist = 0;
        float MinDist = 200;
        TrashNode NearestNode = null;
        foreach (TrashNode node in TrashCounts.Keys)
        {
            dist = Vector3.Distance(SeekingPet.transform.position,node.transform.position);
            if (dist < MinDist)
            {
                MinDist = dist;
                NearestNode = node;
            }
        }

        if (NearestNode != null)
        {
            NearestNode.PetIncoming(SeekingPet);
            SeekingPet.GetComponent<PetBrain>().GoTo(NearestNode.gameObject);
        }
    }

    public void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            TempDate = DateTime.Now;
        }
        else if(TempDate != DateTime.MinValue)
        {
            AddTrashAmount((int)DateTime.Now.Subtract(TempDate).TotalSeconds);
        }
    }

    private void AddTrashAmount(int SecondsPassed)
    {
        int NewAmount = (int)(SecondsPassed / TrashSpawnRate) + GetTrashAmount();
        if (NewAmount > MaxCapacity * TrashNodeNum)
            NewAmount = MaxCapacity * TrashNodeNum;
        while (GetTrashAmount()<NewAmount)
        {
            AddTrash();
        }
    }

    public int GetTrashAmount()
    {
        int ToReturn = 0;
        if (TrashCounts == null)
            TrashCounts = new Hashtable();
        foreach (var TrashCount in TrashCounts.Values)
        {
            ToReturn += (int)TrashCount;
        }
        return ToReturn;
    }

    public void SetTrashAmountOnLoad(int count)
    {
        DateTime CurrentDate = DateTime.Now;
        long temp = Convert.ToInt64(PlayerPrefs.GetString("LastSave"));
        DateTime OldDate = DateTime.FromBinary(temp);
        TimeSpan difference = CurrentDate.Subtract(OldDate);
        print("Difference - "+difference.TotalSeconds);
        print("count - "+count);
        count += (int)(difference.TotalSeconds / TrashSpawnRate);
        if (count > MaxCapacity * TrashNodeNum)
            count = MaxCapacity * TrashNodeNum;
        while (count>GetTrashAmount())
        {
            AddTrash();
        }
    }

    public void AddTrash()
    {
        if (availableTrashNodes.Count > 0)
        {
            rnd = Randomizer.getInstance();
            int index = rnd.getRandom(availableTrashNodes.Count);
            if(TrashCounts == null)
                TrashCounts = new Hashtable();
            if (TrashCounts.ContainsKey(availableTrashNodes[index]))
            {
                availableTrashNodes[index].AddTrash();
                TrashCounts[availableTrashNodes[index]] = 1 + (int)TrashCounts[availableTrashNodes[index]];
                
            }
            else
            {
                TrashCounts.Add(availableTrashNodes[index], 1);
                availableTrashNodes[index].AddTrash();
            }
            if ((int)TrashCounts[availableTrashNodes[index]] == MaxCapacity)
                availableTrashNodes.Remove(availableTrashNodes[index]);
        }
    }


    // Update is called once per frame
    void Update()
    {
        TrashCapacity = (float)GetTrashAmount() / (MaxCapacity*TrashNodeNum);
        if (Player.Animals.Count > 0 && Time.time >= LastSpawn + TrashSpawnRate)
        {
            AddTrash();
            LastSpawn = Time.time;
        }
    }
}
