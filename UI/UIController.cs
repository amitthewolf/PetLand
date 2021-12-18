using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TrashMaster TM;
    public Button FndTrashButton;
    public TMP_Text BValue;
    public Image HappynessBar;
    public Image CameraFilter;
    public GameObject PetIconLocation;
    public List<GameObject> PetIconPrefabs;
    public List<Sprite> AnimalIcons;
    public GameObject PetButtons;
    public GameObject TutorialArrow;
    private bool TrashButtonDisabled;
    private GameObject TempIcon;
    private GameObject TempTarget;
    // Start is called before the first frame update
    void OnEnable()
    {
        EventManager.PetReachedTrashNode += DisableTrashButton;
        EventManager.TrashEmptied += DelayedEnable;
    }
    
    
    private void OnDestroy()
    {
        EventManager.PetReachedTrashNode -= DisableTrashButton;
        EventManager.TrashEmptied -= DelayedEnable;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Player.Animals.Count > 1)
            PetButtons.SetActive(true);
        if(CameraController.TargetPet && (TempTarget != CameraController.TargetPet))
            CheckTargetPet();
        HappynessBar.fillAmount = 1f - TrashMaster.TrashCapacity;
        Color TempColor = CameraFilter.color;
        TempColor.a = 0.5f * TrashMaster.TrashCapacity;
        CameraFilter.color = TempColor;
        BValue.text = Player.Budget.ToString();
        if (TrashButtonDisabled || TM.GetTrashAmount() == 0)
            FndTrashButton.interactable = false;
        else
        {
            FndTrashButton.interactable = true;
        }
    }

    private void CheckTargetPet()
    {
        if(TempIcon)
            Destroy(TempIcon);
        TempTarget = CameraController.TargetPet;
        Animal pet = TempTarget.GetComponent<PetBrain>().animal;
        TempIcon = Instantiate(PetIconPrefabs[pet.Level-1],PetIconLocation.transform);
        TempIcon.transform.position = PetIconLocation.transform.position;
        TempIcon.transform.localScale = TempIcon.transform.localScale * 6.5f;
        switch (pet.getSpecies())
        {
            case Animal.Species.Rabbit:
                TempIcon.GetComponent<Image>().sprite = AnimalIcons[0];
                break;
            case Animal.Species.Duck:
                TempIcon.GetComponent<Image>().sprite = AnimalIcons[1];
                break;
            case Animal.Species.Llama:
                TempIcon.GetComponent<Image>().sprite = AnimalIcons[2];
                break;
        }
    }

    public void DisableTrashButton(object sender, EventArgs args)
    {
        FndTrashButton.interactable = false;
        TrashButtonDisabled = true;
    }
    
    private IEnumerator EnableTrashButton()
    {
        yield return new WaitForSeconds(1f);
        FndTrashButton.interactable = true;
        TrashButtonDisabled = false;
    }
    
    public void DelayedEnable(object sender, EventArgs args)
    {
        StartCoroutine("EnableTrashButton");
    }
}
