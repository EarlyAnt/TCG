using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCapture : MonoBehaviour
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private Text status;
    [SerializeField]
    private Text buttonText;
    [SerializeField]
    private bool autoClearFolder = true;
    private int captureTimes = 0;
    private DateTime startTime = DateTime.Now;
    private bool isRunning = false;
    private string filePath
    {
        get
        {
            return string.Format("{0}/screen_capture", Application.persistentDataPath);
        }
    }
    private string fileName
    {
        get
        {
            return string.Format("{0}/screen_capture_{1}.png", this.filePath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"));
        }
    }
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        if (!Directory.Exists(this.filePath))
            Directory.CreateDirectory(this.filePath);
    }
    /************************************************自 定 义 方 法************************************************/
    public void Run()
    {
        this.isRunning = !this.isRunning;
        if (this.isRunning)
        {
            this.buttonText.text = "Stop";
            if (Directory.Exists(this.filePath))
            {
                foreach (string file in Directory.GetFiles(this.filePath))
                {
                    File.Delete(file);
                }
            }

            this.StopAllCoroutines();
            this.StartCoroutine(this.CaptureScreen());

        }
        else
        {
            this.buttonText.text = "Start";
            this.StopAllCoroutines();
        }
    }
    private IEnumerator CaptureScreen()
    {
        this.startTime = DateTime.Now;
        this.captureTimes = 1;

        while (this.isRunning)
        {
            UnityEngine.ScreenCapture.CaptureScreenshot(this.fileName);
            this.status.text = string.Format("remain: {0:f1}s, capture times: {1}", (DateTime.Now - this.startTime).TotalSeconds, this.captureTimes);
            this.captureTimes += 1;
            yield return null;
        }
    }
}
