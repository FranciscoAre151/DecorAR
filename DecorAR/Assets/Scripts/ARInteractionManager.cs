using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARInteractionManager : MonoBehaviour
{
    [SerializeField] private Camera aRCamera;
    [SerializeField] private ARAnchorManager aRAnchorManager;
    private ARRaycastManager aRRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private GameObject aRPointer;
    private GameObject item3DModel;
    private GameObject itemSelected;

    private bool isInitialPosition;
    private bool isOverUI;
    private bool isOver3DModel;
    private bool isRotating;

    private List<ItemButtonManager> placedObjects = new List<ItemButtonManager>();


    private Vector2 initialTouchPos;

    // Variables para el panel de escalado
    [SerializeField] private GameObject measurePanel;
   /* [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider depthSlider; */
    
    [SerializeField] private TMP_Dropdown objectDropdown; // Referencia al Dropdown
    [SerializeField] private TMP_Text textHeight;
    [SerializeField] private TMP_Text textWidth;
    [SerializeField] private TMP_Text textDepth;


    private Vector3 originalScale;


    private ItemButtonManager itemButtonManager;

    public void SetItemButtonManager(ItemButtonManager manager)
    {
        itemButtonManager = manager;
    }

    public GameObject Item3DModel
    {
        set
        {
            item3DModel = value;

            item3DModel.transform.position = aRPointer.transform.position;
            item3DModel.transform.parent = aRPointer.transform;
            isInitialPosition = true;
            originalScale = item3DModel.transform.localScale; // Guardar la escala original del modelo


            UpdateValueTexts();
            measurePanel.gameObject.SetActive(true);
        }
    }



    void Start()
    {

        aRPointer = transform.GetChild(0).gameObject;
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        GameManager.instance.OnMainMenu += SetItemPosition;
        //GameManager.instance.OnDropdownSelectionChanged += OnDropdownValueChanged;
        //Inicializar el panel y el botón
        measurePanel.gameObject.SetActive(false);  // Ocultar el botón al inicio
       
        objectDropdown.gameObject.SetActive(true);

    }

    void Update()
    {
        if (isInitialPosition)
        {
            Vector2 middlePointScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            aRRaycastManager.Raycast(middlePointScreen, hits, TrackableType.Planes);
            if (hits.Count > 0)
            {
                transform.position = hits[0].pose.position;
                transform.rotation = hits[0].pose.rotation;
                aRPointer.SetActive(true);
                isInitialPosition = false;
            }
        }

        if (Input.touchCount > 0)
        {
            Touch touchOne = Input.GetTouch(0);
            if (touchOne.phase == TouchPhase.Began)
            {
                var touchPosition = touchOne.position;
                isOverUI = isTapOverUI(touchPosition);
                isOver3DModel = isTapOver3DModel(touchPosition);
                
            }


            if (touchOne.phase == TouchPhase.Moved && !isRotating)
            {
                if (aRRaycastManager.Raycast(touchOne.position, hits, TrackableType.Planes))
                {
                    Pose hitPose = hits[0].pose;
                    if (!isOverUI)
                    {
                        transform.position = hitPose.position;
                    }

                }
            }

            if (Input.touchCount == 2)
            {
                Touch touchTwo = Input.GetTouch(1);
                if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
                {
                    initialTouchPos = touchTwo.position - touchOne.position;
                    isRotating = true;
                }

                if (touchOne.phase == TouchPhase.Moved || touchTwo.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPos = touchTwo.position - touchOne.position;
                    float angle = Vector2.SignedAngle(initialTouchPos, currentTouchPos);
                    item3DModel.transform.rotation = Quaternion.Euler(0, item3DModel.transform.eulerAngles.y - angle, 0);
                    initialTouchPos = currentTouchPos;
                }
            }
            else
            {
                isRotating = false;
            }

            if (isOver3DModel && item3DModel == null && !isOverUI)
            {
                GameManager.instance.ARPosition();
                item3DModel = itemSelected;
                itemSelected = null;
                aRPointer.SetActive(true);
                transform.position = item3DModel.transform.position;
                item3DModel.transform.parent = aRPointer.transform;
            }
        }
    }

    private void UpdateValueTexts()
    {
        if (item3DModel != null)
        {
            textHeight.text = itemButtonManager.ItemHeight; 
            textWidth.text = itemButtonManager.ItemWidth;
            textDepth.text = itemButtonManager.ItemDepth;
        }
    }

    private bool isTapOver3DModel(Vector2 touchPosition)
    {
        Ray ray = aRCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit3DModel))
        {
            if (hit3DModel.collider.CompareTag("Item"))
            {
                itemSelected = hit3DModel.transform.gameObject;
                return true;
            }
        }
        return false;
    }

    private bool isTapOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);
        return result.Count > 0;
    }

    private void SetItemPosition()
    {
        if (item3DModel != null)
        {
            item3DModel.transform.parent = null;
            aRPointer.SetActive(false);
            item3DModel = null;

            measurePanel.gameObject.SetActive(false);
        }
    }

  

    public void DeleteItem()
    {

        Destroy(item3DModel);

        aRPointer.SetActive(false);
        GameManager.instance.MainMenu();

        measurePanel.gameObject.SetActive(false);
    }

    private void UpdateDropdownOptions()
    {
        objectDropdown.ClearOptions(); 

        List<string> objectNames = new List<string>();
        for (int i = 0; i < placedObjects.Count; i++)
        {
            if (placedObjects[i].Item3DModel != null)
            {
                objectNames.Add($"Objeto {i + 1}: {placedObjects[i].ItemName}");
            }
            else
            {
                objectNames.Add($"Objeto {i + 1}: Sin Nombre");
            }
        }

        if(objectNames.Count > 0)
        {
            objectDropdown.gameObject.SetActive(true);
        }else
        {
            objectDropdown.gameObject.SetActive(false);
        }

        objectDropdown.AddOptions(objectNames); 
    }

    public void ConfirmPlacement()
    {
        if (item3DModel == null) return;

        ARAnchor anchor = aRAnchorManager.AddAnchor(new Pose(item3DModel.transform.position, item3DModel.transform.rotation));
        if (anchor == null)
        {
            Debug.LogError("No se pudo crear un anchor en la posición actual.");
            return;
        }

        item3DModel.transform.parent = anchor.transform;

        placedObjects.Add(new ItemButtonManager
        {
            Item3DModel = item3DModel,
            ItemName = itemButtonManager.ItemName,
            Anchor = anchor
        });
  
        UpdateDropdownOptions();

        GameManager.instance.MainMenu();

        Debug.Log("Objeto confirmado y anclado en la posición.");
    }

    
    public void DeleteSelectedItem()
    {
        int index = objectDropdown.value;
        Debug.Log($"Índice seleccionado: {index}");

        if (index >= 0 && index < placedObjects.Count)
        {
            
            ItemButtonManager selectedItemManager = placedObjects[index];

            if (selectedItemManager.Item3DModel != null)
            {
                ARAnchor anchor = selectedItemManager.Item3DModel.GetComponent<ARAnchor>();
                if (anchor != null)
                {
                    Destroy(anchor); 
                }
                Destroy(selectedItemManager.Item3DModel); 
                
            }

            placedObjects.RemoveAt(index);

            UpdateDropdownOptions();

        }
    }
}
