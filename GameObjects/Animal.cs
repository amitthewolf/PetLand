using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Animal
{
    public float Happiness;
    public float Hunger;
    public float Thirst;
    public int Level;
    public Species species;

    public Animal(Species s)
    {
        Level = 1;
        Happiness = 100f;
        Hunger = 100f;
        Thirst = 100f;
        species = s;
    }
    
    public float getHappiness()
    {
        return Happiness;
    }
    public float getHunger()
    {
        return Hunger;
    }
    public int getLevel()
    {
        return Level;
    }

    public float getThirst()
    {
        return Thirst;
    }
    public float getWorth()
    {
        return Level;
    }
    
    public Species getSpecies()
    {
        return species;
    }

    public void LifeTick()
    {
        HungerTick();
        HappinessTick();
        ThirstTick();
    }

    private void ThirstTick()
    {
        throw new System.NotImplementedException();
    }

    private void HappinessTick()
    {
        throw new System.NotImplementedException();
    }

    private void HungerTick()
    {
        throw new System.NotImplementedException();
    }

    public void setHappiness(float newValue)
    {
        Happiness = newValue;
    }

    public void setHunger(float newValue)
    {
        Hunger = newValue;
    }

    public void setLevel(int newValue)
    {
        Level = newValue;
    }
    [Serializable]
    public enum Species
    {
        Rabbit,
        Duck,
        Cat,
        Dog,
        Llama,
        Horse,
    }

}
