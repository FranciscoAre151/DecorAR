using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public event Action OnMainMenu;
    public event Action OnItemsMenu;
    public event Action OnARPosition;
    public event Action OnInicio;

    public static GameManager instance;

    //public event Action<int> OnDropdownSelectionChanged;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        Inicio();
    }

    public void Inicio()
    {
        OnInicio?.Invoke();

    }

    public void MainMenu()
    {
        OnMainMenu?.Invoke();

    }

    public void ItemsMenu()
    {
        OnItemsMenu?.Invoke();
    }

    public void ARPosition()
    {
        OnARPosition?.Invoke();
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    //public void TriggerDropdownSelectionChanged(int index)
    //{
      //  OnDropdownSelectionChanged?.Invoke(index);
    //}

}