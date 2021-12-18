using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public List<GameObject> TutorialWindows;

    private int counter;
    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
        if (Player.Animals.Count > 0)
            NextWindow();
    }

    public void NextWindow()
    {
        TutorialWindows[counter].SetActive(false);
        counter++;
        if(counter < TutorialWindows.Count)
            TutorialWindows[counter].SetActive(true);
    }
    
    public void TutorialEnd()
    {
        Player.Tutorial = false;
    }

    private void Update()
    {
        print(Player.Tutorial);
        if(!Player.Tutorial)
            TutorialWindows[counter].SetActive(false);
        else
            TutorialWindows[counter].SetActive(true);
    }
}
