using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MazeObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private AudioSource _audioSource;
    
    [Space]
    [SerializeField] private float _animationDuration;
    [SerializeField] private AnimationCurve _showAnimationCurve;
    [SerializeField] private AnimationCurve _hideAnimationCurve;
    [SerializeField] private AudioClip _hideAudio;

    [Space]
    [SerializeField] private ParticleSystem _hideParticleSystem;
    
    public Vector2Int position => _position;
    public GroundType groundType => _groundType;

    private GroundType _groundType;
    private Vector2Int _position;
    private float _currentT;
    private AnimationType _animationType;
    private bool _animating;

    private enum AnimationType
    {
        Show,
        Hide
    }
    
    public class Pool : MonoMemoryPool<GroundType, Vector2Int, Color, MazeObject>
    {
        protected override void Reinitialize(GroundType groundType, Vector2Int position, Color color, MazeObject item)
        {
            item.Reinitialize(groundType, position, color);    
        }
    }

    public void UpdateObject(GroundType type, Color color)
    {
        _groundType = type;
        if (type != GroundType.Empty)
        {
            _spriteRenderer.color = color;
            var main = _hideParticleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(color.r, color.g, color.b, 0.0f));
        } 
    }

    public void Show(bool withRandom)
    {
        _animating = true;
        _animationType = AnimationType.Show;
        _currentT = 0;
    }
    
    public void Hide(bool withRandom, bool withoutSound)
    {
        if (_animating || Mathf.Approximately(transform.localScale.x, 0.0f))
        {
            return;
        }
        
        _animating = true;
        _animationType = AnimationType.Hide;
        _currentT = 0;
        
        _hideParticleSystem.Play();

        if (!withoutSound)
        {
            _audioSource.clip = _hideAudio;
            _audioSource.Play();
        }
    }

    protected void Update()
    {
        if (_currentT > _animationDuration)
        {
            Scale(1.0f);
            _animating = false;
            return;
        }

        var t = _currentT / _animationDuration;

        Scale(t);

        _currentT += Time.deltaTime;
    }

    private void Scale(float t)
    {
        var size = transform.localScale;
        switch (_animationType)
        {
            case AnimationType.Show:
                var showScale = Mathf.Lerp(0, 1, _showAnimationCurve.Evaluate(t));
                size.x = showScale;
                size.y = showScale;
                break;
            case AnimationType.Hide:
                var hideScale = Mathf.Lerp(1, 0, _hideAnimationCurve.Evaluate(t)); 
                size.x = hideScale;
                size.y = hideScale;
                break;
        }
        transform.localScale = size;
    }
    
    private void Reinitialize(GroundType groundType, Vector2Int position, Color color)
    {
        _groundType = groundType;
        _position = position;
        transform.localScale = new Vector2(0, 0);
        
        _spriteRenderer.color = color;
        var main = _hideParticleSystem.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color, new Color(color.r, color.g, color.b, 0.0f));

        _animating = false;
    }
}
