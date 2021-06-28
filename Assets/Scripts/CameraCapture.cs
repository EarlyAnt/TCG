using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    [SerializeField]
    private Camera cardCamera;
    [SerializeField]
    private Rect rect;
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
            return string.Format("{0}/camera_capture", Application.persistentDataPath);
        }
    }
    private string fileName
    {
        get
        {
            return string.Format("{0}/camera_capture_{1}.png", this.filePath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"));
        }
    }

    private void Start()
    {
    }

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
            this.StartCoroutine(this.CaptureCamera());

        }
        else
        {
            TimeSpan timeSpan = DateTime.Now - this.startTime;
            this.status.text = string.Format("remain: {0:f1}s, capture times: {1}, capture times per second: {2:f1}",
                                              timeSpan.TotalSeconds, this.captureTimes, this.captureTimes / timeSpan.TotalSeconds);
            this.buttonText.text = "Start";
            this.StopAllCoroutines();
        }
    }

    private IEnumerator CaptureCamera()
    {
        this.startTime = DateTime.Now;
        this.captureTimes = 1;

        while (this.isRunning)
        {
            //创建一个RenderTexture对象
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            //临时设置相关相机的targetTexture, 并手动渲染相关相机
            this.cardCamera.targetTexture = renderTexture;
            this.cardCamera.Render();
            //激活这个rt, 并从中中读取像素。  
            RenderTexture.active = renderTexture;
            Texture2D screenShot = new Texture2D((int)this.rect.width, (int)this.rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(this.rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
            screenShot.Apply();
            //重置相关参数，以使用this.cardCamera继续在屏幕上显示
            this.cardCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(renderTexture);
            //最后将这些纹理数据，成一个png图片文件
            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.File.WriteAllBytes(this.fileName, bytes);

            this.status.text = string.Format("remain: {0:f1}s, capture times: {1}", (DateTime.Now - this.startTime).TotalSeconds, this.captureTimes);
            this.captureTimes += 1;
            yield return null;
        }
    }
}
