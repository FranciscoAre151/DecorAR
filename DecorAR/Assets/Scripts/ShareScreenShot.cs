using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShareScreenShot : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private TMP_Dropdown objectDropdown;

    private ARPointCloudManager aRPointCloudManager;

    // Start is called before the first frame update
    void Start()
    {
        aRPointCloudManager = FindObjectOfType<ARPointCloudManager>();  

    }

    public void TakeScreenShot()
    {
        TurnOnOffARCOntents();
        StartCoroutine(TakeScreenshotAndShare());
        
    }

    private void TurnOnOffARCOntents()
    {
        var points = aRPointCloudManager.trackables;
        foreach(var point in points)
        {
            point.gameObject.SetActive(!point.gameObject.activeSelf);
        }
        mainMenuCanvas.SetActive(!mainMenuCanvas.activeSelf);
        objectDropdown.gameObject.SetActive(false);

    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hey!... Mirá como quedaría este mueble en mi habitación!")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();
        TurnOnOffARCOntents();
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}
