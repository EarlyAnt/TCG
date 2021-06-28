﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSaver : MonoBehaviour
{
    /************************************************属性与变量命名************************************************/
    [SerializeField]
    private Image imageUp;
    [SerializeField]
    private Image imageDown;
    [SerializeField, Range(1f, 5f)]
    private float interval = 1f;
    [SerializeField]
    private List<Sprite> imageList;
    private int index;
    /************************************************Unity方法与事件***********************************************/
    private void Start()
    {
        this.imageUp.sprite = this.imageList[0];
        this.imageDown.sprite = this.imageList[1];
        this.index = 1;

        this.InvokeRepeating("RefreshScreen", 0f, this.interval);
    }
    /************************************************自 定 义 方 法************************************************/
    private void RefreshScreen()
    {
        this.imageDown.sprite = this.imageList[this.index];
        int nextIndex = (this.index + 1) % this.imageList.Count;
        this.imageUp.DOFade(0f, 1f).onComplete = () =>
        {
            this.imageUp.sprite = this.imageList[this.index];
            this.imageUp.DOFade(1f, 0f);
            this.index = nextIndex;
        };
    }
}
