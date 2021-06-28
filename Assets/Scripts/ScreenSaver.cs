using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSaver : MonoBehaviour
{
    [SerializeField]
    private Image imageUp;
    [SerializeField]
    private Image imageDown;
    [SerializeField, Range(1f, 5f)]
    private float interval = 1f;
    [SerializeField]
    private List<Sprite> imageList;
    private int index;

    private void Start()
    {
        this.imageUp.sprite = this.imageList[0];
        this.imageDown.sprite = this.imageList[1];
        this.index = 1;

        this.InvokeRepeating("RefreshScreen", 0f, this.interval);
    }

    private void RefreshScreen()
    {
        int nextIndex = (this.index + 1) % this.imageList.Count;
        this.imageUp.DOFade(0f, 0.5f).onComplete = () =>
        {
            this.imageUp.sprite = this.imageDown.sprite;
            this.imageDown.sprite = this.imageList[nextIndex];
            this.index = nextIndex;
        };
    }
}
