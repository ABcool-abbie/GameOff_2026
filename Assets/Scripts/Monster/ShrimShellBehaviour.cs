using DG.Tweening;
using UnityEngine;

public class ShrimpShellBehaviour : MonoBehaviour
{
    [SerializeField] Sprite ownerNewSprite;
    [SerializeField] float fallOffTime = 1f;
    [SerializeField] private float fallOffSpeed = 0.2f;
    [SerializeField] private float fallOffOscillateSpeed = .8f;
    private float _animatedTime;
    private Monster _owner;
    private Vector3 _lastWorldPos;

    public void Start()
    {
        _owner = GetComponentInParent<Monster>();
        _owner.SpriteRenderer.sprite = ownerNewSprite;

        _animatedTime = 0f;

        transform.DOShakePosition(fallOffTime, strength: 0.1f, vibrato: 100);
        transform.DOShakeRotation(fallOffTime, randomness: 5f);
    }

    public void Update()
    {
        bool fallOffThisFrame = _animatedTime < fallOffTime && _animatedTime + Time.deltaTime >= fallOffTime;
        _animatedTime += Time.deltaTime;
        if (fallOffThisFrame)
        {
            _lastWorldPos = transform.position;
            transform.DOKill();
            transform.DORotate(new Vector3(0f, 0f, 360f), 5f).SetLoops(0);
        }

        if (_animatedTime >= fallOffTime)
        {
            transform.position = _lastWorldPos 
                + Vector3.down * _animatedTime * fallOffSpeed 
                + Vector3.right * Mathf.Sin(_animatedTime * fallOffOscillateSpeed) * fallOffSpeed;
        }
    }
}
