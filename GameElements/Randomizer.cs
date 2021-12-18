using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Randomizer
{
    private Random Generator;
    private static Randomizer instance;

    public static Randomizer getInstance()
    {
        if (instance == null)
            instance = new Randomizer();
        return instance;
    }

    public Randomizer()
    {
        Generator = new Random();
    }

    public int getRandom(int max)
    {
        return Generator.Next(max);
    }
}
