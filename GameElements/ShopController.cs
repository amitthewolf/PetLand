using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public List<Sprite> AnimalIcons;
    public List<GameObject> PetIconPrefabs;
    public List<GameObject> PetPrefabs;
    public GameObject ScrollPanel;
    public GameObject PetLocation;
    public Button LevelUpButton;
    public TMP_Text LevelUpPrice;
    public Button BuyButton;
    public TMP_Text Budget;
    private Animal ChosenPet = null;
    private GameObject SpawnedPet = null;
    private GameObject ChosenPetIcon;
    private bool Browsing = false;


    void Start()
    {
        Player.Animals.ForEach(pet => CreateIcon(pet));
    }

    void Update()
    {
        Budget.text = Player.Budget.ToString();
        if(ChosenPet==null || Browsing)
            LevelUpButton.interactable = false;
        else if (ChosenPet.Level < 3)
        {
            LevelUpButton.interactable = true;
            LevelUpPrice.text = (ChosenPet.Level * (50 + 50 * (float)Math.Floor((double)(int)ChosenPet.species/2))).ToString();
        }
        else
        {
            LevelUpButton.interactable = false;
            LevelUpPrice.text = "Max";
        }
    }

    private void CreateIcon(Animal pet)
    {
        GameObject NewIcon = Instantiate(PetIconPrefabs[pet.Level-1],ScrollPanel.transform);
        switch (pet.getSpecies())
        {
            case Animal.Species.Rabbit:
                NewIcon.GetComponent<Image>().sprite = AnimalIcons[0];
                break;
            case Animal.Species.Duck:
                NewIcon.GetComponent<Image>().sprite = AnimalIcons[1];
                break;
            case Animal.Species.Llama:
                NewIcon.GetComponent<Image>().sprite = AnimalIcons[2];
                break;
        }
        NewIcon.GetComponent<Button>().onClick.AddListener(delegate { ChoosePet(pet, NewIcon); });
        ChosenPetIcon = NewIcon;
    }

    public void ChoosePet(Animal pet, GameObject Icon)
    {
        Browsing = false;
        BuyButton.gameObject.SetActive(false);
        ChosenPet = pet;
        ChosenPetIcon = Icon;
        SpawnPet(pet);
    }

    private bool CanAfford(int Price)
    {
        if (Player.Budget >= Price)
        {
            Player.RemoveBudget(Price);
            return true;
        }
        return false;
    }
    private void SpawnPet(Animal pet)
    {
        if (SpawnedPet != null)
        {
            Destroy(SpawnedPet);
        }
        switch (pet.getSpecies())
        {
            case Animal.Species.Llama:
                SpawnedPet = Instantiate(PetPrefabs[0],PetLocation.transform);
                SpawnedPet.GetComponent<PetBrain>().setAnimal(pet);
                SpawnedPet.GetComponent<PetBrain>().enabled = false;
                SpawnedPet.transform.position = PetLocation.transform.position;
                break;
            case Animal.Species.Duck:
                SpawnedPet = Instantiate(PetPrefabs[1],PetLocation.transform);
                SpawnedPet.GetComponent<PetBrain>().setAnimal(pet);
                SpawnedPet.GetComponent<PetBrain>().enabled = false;
                SpawnedPet.transform.position = PetLocation.transform.position;
                break;
            case Animal.Species.Rabbit:
                SpawnedPet = Instantiate(PetPrefabs[2],PetLocation.transform);
                SpawnedPet.GetComponent<PetBrain>().setAnimal(pet);
                SpawnedPet.GetComponent<PetBrain>().enabled = false;
                SpawnedPet.transform.position = PetLocation.transform.position;
                break;
            case Animal.Species.Cat:
                SpawnedPet = Instantiate(PetPrefabs[2],PetLocation.transform);
                SpawnedPet.GetComponent<PetBrain>().setAnimal(pet);
                SpawnedPet.GetComponent<PetBrain>().enabled = false;
                SpawnedPet.transform.position = PetLocation.transform.position;
                break;
        }
    }

    public void LevelChosenPet()
    {
        if(CanAfford((int)(ChosenPet.Level * (50 + 50 * (float)Math.Floor((double)(int)ChosenPet.species/2)))))
        {
            if (ChosenPet.Level == 1)
            {
                SpawnedPet.transform.localScale = SpawnedPet.transform.localScale*2f;
            }
            else
            {
                SpawnedPet.transform.localScale = SpawnedPet.transform.localScale*1.5f;
            }
            ChosenPet.Level += 1;
            Destroy(ChosenPetIcon);
            CreateIcon(ChosenPet);
            AudioManager.instance.LevelUpSound();
            SavingSystem.SaveAnimalProgress(Player.Animals);
        }
    }

    public void BuyNewPet()
    {
        if (ChosenPet.species == Animal.Species.Llama)
        {
            if (CanAfford(25))
            {
                Player.Animals.Add(ChosenPet);
                CreateIcon(ChosenPet);
                SavingSystem.SaveAnimalProgress(Player.Animals);
            }
        }
        else if(CanAfford(5))
        {
            Player.Animals.Add(ChosenPet);
            CreateIcon(ChosenPet);
            SavingSystem.SaveAnimalProgress(Player.Animals);
        }
        
    }
    
    public void BrowseRabbit()
    {
        Browsing = true;
        ChosenPet = new Animal(Animal.Species.Rabbit);
        SpawnPet(ChosenPet);
        BuyButton.gameObject.SetActive(true);
    }
    
    public void BrowseDuck()
    {
        Browsing = true;
        ChosenPet = new Animal(Animal.Species.Duck);
        SpawnPet(ChosenPet);
        BuyButton.gameObject.SetActive(true);
    }
    
    public void BrowseLlama()
    {
        Browsing = true;
        ChosenPet = new Animal(Animal.Species.Llama);
        SpawnPet(ChosenPet);
        BuyButton.gameObject.SetActive(true);
    }
}
