using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FrameCapture : MonoBehaviour
{
    public bool saveToDisk = false;

    public string filePath = "";
    public string filePrefix = "";

    int frameIndex = 0;

    public void SaveFrame()
    {
        if (!saveToDisk) return;

        Debug.Log("Frame saved:" + frameIndex.ToString());

        SaveFrameToDisk(frameIndex);
        frameIndex++;
    }

    void SaveFrameToDisk(int frameIndex)
    {
        string path = Path.Combine(filePath, filePrefix + "_" + frameIndex.ToString("000000") + ".jpg");
        ScreenCapture.CaptureScreenshot(path);
    }
}
