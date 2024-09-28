using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float aspectRatio = screenWidth / screenHeight;
        Debug.Log("Screen width: " + screenWidth);
        Debug.Log("Screen height: " + screenHeight);
        Debug.Log("Aspect ratio: " + aspectRatio);
        if (aspectRatio < 0.5f) {
            Camera.main.orthographicSize = 15;
            Camera.main.transform.position += new Vector3(0, 0.8f, 0);
        }
    }
}
