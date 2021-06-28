using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    /************************************************属性与变量命名************************************************/
    #region 页面UI组件
    [SerializeField]
    private Camera cardCamera;
    [SerializeField]
    private Dropdown dropdown;
    [SerializeField]
    private List<Vector2> sizeList;
    [SerializeField]
    private Text fps;
    [SerializeField]
    private Text status;
    [SerializeField]
    private Text buttonText;
    [SerializeField]
    private bool autoClearFolder = true;
    #endregion
    #region 其他变量
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
    private int rectIndex = 0;
    private List<Rect> rectList;
    private Rect rect
    {
        get
        {
            return this.rectIndex >= 0 && this.rectIndex < this.rectList.Count ? this.rectList[this.rectIndex] : Rect.zero;
        }
    }
    #endregion
    /************************************************Unity方法与事件***********************************************/
    private void Awake()
    {
        Application.targetFrameRate = 300;
    }
    private void Start()
    {
        if (!Directory.Exists(this.filePath))
            Directory.CreateDirectory(this.filePath);

        List<string> solutions = new List<string>();
        this.rectList = new List<Rect>();
        foreach (var size in this.sizeList)
        {
            float left = (Screen.width - size.x) / 2;
            float top = (Screen.height - size.y) / 2;
            this.rectList.Add(new Rect(left, top, size.x, size.y));
            solutions.Add(string.Format("[{0}] {1}*{2}", solutions.Count + 1, size.x, size.y));
        }

        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(solutions);
        this.dropdown.onValueChanged.AddListener(this.OnDropdownValueChanged);
    }
    private void Update()
    {
        this.fps.text = string.Format("fps: {0:f2}", 1f / Time.smoothDeltaTime);
    }
    private void OnDestroy()
    {
        this.dropdown.onValueChanged.RemoveListener(this.OnDropdownValueChanged);
    }
    /************************************************自 定 义 方 法************************************************/
    public void ClearFolder()
    {
        if (Directory.Exists(this.filePath))
        {
            foreach (string file in Directory.GetFiles(this.filePath))
            {
                File.Delete(file);
            }
        }
    }
    public void Run()
    {
        this.isRunning = !this.isRunning;
        if (this.isRunning)
        {
            this.buttonText.text = "Stop";
            this.ClearFolder();
            this.StopCoroutine("CaptureCamera");
            this.StartCoroutine(this.CaptureCamera());

        }
        else
        {
            this.buttonText.text = "Start";
            this.StopCoroutine("CaptureCamera");
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    private IEnumerator CaptureCamera()
    {
        this.startTime = DateTime.Now;
        this.captureTimes = 0;

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
            this.StartCoroutine(this.WriteFile(screenShot.EncodeToPNG()));

            TimeSpan timeSpan = DateTime.Now - this.startTime;
            this.status.text = string.Format("remain: {0:f1}s, capture times: {1}(per second: {2:f1})",
                                             timeSpan.TotalSeconds, this.captureTimes, this.captureTimes / timeSpan.TotalSeconds);
            this.captureTimes += 1;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator WriteFile(byte[] datas)
    {
        File.WriteAllBytes(this.fileName, datas);
        yield return null;
    }
    private void OnDropdownValueChanged(int value)
    {
        this.rectIndex = value;
    }
}
