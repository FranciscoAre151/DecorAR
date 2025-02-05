using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

public class ItemButtonManager : MonoBehaviour
{
    private string itemName;
    private string itemDescription;
    private Sprite itemImage;
    private GameObject item3DModel;
    private ARInteractionManager interactionManager;
    private string category;
    private string urlBundleModel;
    private string itemHeight;
    private string itemWidth;
    private string itemDepth;
    private RawImage imageBundle;
    private ARAnchor anchor;
    public string ItemName { 
        set 
        { 
            itemName = value;
        }
        get { return itemName; }
    }

   
    public string ItemDescription { set => itemDescription = value; }
    public Sprite ItemImage { set => itemImage = value; }
    public GameObject Item3DModel { set => item3DModel = value; get => item3DModel; }
    public string URLBundleModel { set => urlBundleModel = value; }
    public RawImage ImageBundle { get => imageBundle; set => imageBundle = value; }
    public string Category { get => category; set => category = value; }
    public string ItemHeight { get => itemHeight; set => itemHeight = value; }
    public string ItemWidth {get => itemWidth; set => itemWidth = value; }
    public string ItemDepth { get => itemDepth; set => itemDepth = value; }
    public ARAnchor Anchor { get => anchor; set => anchor = value; }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        //transform.GetChild(1).GetComponent<RawImage>().texture = itemImage.texture;
        imageBundle = transform.GetChild(1).GetComponent<RawImage>(); 
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = itemDescription;
        Debug.Log("Height: " + itemHeight);
        Debug.Log("Width: " + itemWidth);
        Debug.Log("Depth: " + itemDepth);

        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = itemHeight;
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = itemWidth;
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = itemDepth;

        var button = GetComponent<Button>();
        button.onClick.AddListener(GameManager.instance.ARPosition);
        button.onClick.AddListener(Create3DModel);

        interactionManager = FindObjectOfType<ARInteractionManager>();
    }

    private void Create3DModel()
    {
        //interactionManager.Item3DModel =  Instantiate(item3DModel);
        StartCoroutine(DownloadAssetBundle(urlBundleModel));
        interactionManager.SetItemButtonManager(this);

    }

    IEnumerator DownloadAssetBundle(string urlAssetBundle)
    {
        UnityWebRequest serverRequest = UnityWebRequestAssetBundle.GetAssetBundle(urlAssetBundle);
        yield return serverRequest.SendWebRequest();
        if(serverRequest.result == UnityWebRequest.Result.Success) 
         {
             AssetBundle model3D = DownloadHandlerAssetBundle.GetContent(serverRequest);
             if(model3D != null) 
             {
                 interactionManager.Item3DModel = Instantiate(model3D.LoadAsset(model3D.GetAllAssetNames()[0]) as GameObject);
             }
             else
             {
                 Debug.Log("Not a valid Assets Bundle");
             }
         }
         else
         {
             Debug.Log("Error :(");
         }
        

       

    }
}
