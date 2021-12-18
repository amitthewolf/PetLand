using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SaveData
{
    public List<Animal> Animals;

    public SaveData(List<Animal> animals)
    {
        Animals = animals;
    }
}
