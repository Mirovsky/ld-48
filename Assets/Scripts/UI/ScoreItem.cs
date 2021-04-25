using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    
    public class Pool : MonoMemoryPool<ScoreItem> { }

    public Color color
    {
        get => _image.color;
        set => _image.color = value;
    }

    public RectTransform rectTransform => _rectTransform;
}
