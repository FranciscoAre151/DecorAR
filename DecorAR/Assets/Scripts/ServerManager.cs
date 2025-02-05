using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Xml.Serialization;
using DG.Tweening.Core.Easing;
using TMPro;
using System.Linq.Expressions;
using System.Reflection;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private string jsonURL;
    [SerializeField] private ItemButtonManager itemButtonManager;
    [SerializeField] private GameObject buttonsContainer;
    [SerializeField] private TMP_Dropdown categoryDropdown;

    [SerializeField] private List<string> categories;


    [Serializable]
    public struct Items
    {
        [Serializable]
        public struct Item
        {
            public string Name;
            public string Description;
            public string URLBundleModel;
            public string Category;
            public string URLImageModel;
            public string ItemHeight;
            public string ItemWidth;
            public string ItemDepth;
        }

        public Item[] items;
    
    }

    public Items newItemsCollection = new Items();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetJsonData());
        InitializeDropdown();

        GameManager.instance.OnItemsMenu += CreateButtons;
    }

    private void CreateButtons()
    {
        ClearButtons();

        foreach (var item in newItemsCollection.items)
        {
            Debug.Log($"Estoy en el foreach. Índice: {categories[categoryDropdown.value]}, Valor:{item.Category}");
            if (categories[categoryDropdown.value] == item.Category)
            {
                ItemButtonManager itemButton;
                itemButton = Instantiate(itemButtonManager, buttonsContainer.transform);
                itemButton.name = item.Name;
                itemButton.ItemName = item.Name;
                itemButton.ItemDescription = item.Description;
                itemButton.URLBundleModel = item.URLBundleModel;
                itemButton.ItemHeight = item.ItemHeight;
                itemButton.ItemWidth = item.ItemWidth;
                itemButton.ItemDepth = item.ItemDepth;
                StartCoroutine(GetBundleImage(item.URLImageModel, itemButton));
            }
            
        }
        //GameManager.instance.OnItemsMenu -= CreateButtons;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void ClearButtons()
    {
        // Eliminar todos los hijos de buttonsContainer
        foreach (Transform child in buttonsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitializeDropdown()
    {
        categoryDropdown.ClearOptions(); // Limpiar opciones previas
        categoryDropdown.AddOptions(categories); // Agregar las categorías al dropdown
    }

    IEnumerator GetJsonData()
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(jsonURL);
        yield return serverRequest.SendWebRequest();

        if(serverRequest.result == UnityWebRequest.Result.Success)
        {
            newItemsCollection = JsonUtility.FromJson<Items>(serverRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error :(");
        }
    }

    IEnumerator GetBundleImage(string urlImage, ItemButtonManager button)
    {
        UnityWebRequest serverRequest = UnityWebRequest.Get(urlImage);
        serverRequest.downloadHandler = new DownloadHandlerTexture();
        yield return serverRequest.SendWebRequest();

        if (serverRequest.result == UnityWebRequest.Result.Success)
        {
            button.ImageBundle.texture = ((DownloadHandlerTexture)serverRequest.downloadHandler).texture;
        }
        else
        {
            Debug.Log("Error :(");
        }
    }
}
