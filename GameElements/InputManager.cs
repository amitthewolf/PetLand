using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private int TrashLayer;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        TrashLayer = LayerMask.GetMask("Trash");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin,ray.direction*20,Color.red);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, TrashLayer))
            {
                print("Trash");
                hit.transform.gameObject.GetComponent<Trash>().Recycle();
            }
        }
    }

    public void ShopScene()
    {
        EventManager.CallFullSave(this,EventArgs.Empty);
        SceneManager.LoadScene("Shop");
    }
    
    public void GardenScene()
    {
        SceneManager.LoadScene("Garden");
    }
}
