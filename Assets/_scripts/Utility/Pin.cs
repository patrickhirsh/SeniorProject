using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Level;
using UnityEngine;
using UnityEngine.UI;


public class Pin : MonoBehaviour
{
    public Passenger Passenger;
    public Transform Sprite;
    public LineRenderer LineRenderer;
    public Text SelectionNumber;
    public Image RadialTimerImg;

    private bool _selected;
    private bool _hover;
    private Camera _camera;
    private float _y;
    private Sequence _selectionSequence;
    private Sequence _hoverSequence;

    #region Unity Methods

    private void Awake()
    {
        _camera = Camera.main;
        SelectionNumber.DOFade(0, 0);
        PlayerVehicleManager.Instance.HoverChanged.AddListener(HandleHoverChange);

        _hoverSequence = DOTween.Sequence();
        _hoverSequence.Append(transform.DOMoveY(3f, .2f).SetRelative(true));
        _hoverSequence.SetAutoKill(false).Pause();
    }

    private void HandleHoverChange(GameObject go)
    {
        var isHovering = go == gameObject && !_selected;
        SetHover(isHovering);
    }

    private void Update()
    {
        if (Sprite.gameObject.activeInHierarchy)
        {
            Sprite.LookAt(_camera.transform);
        }

        if (SelectionNumber.gameObject.activeInHierarchy)
        {
            SelectionNumber.transform.parent.LookAt(_camera.transform);
        }

        ComputeLine();
    }

    private void ComputeLine()
    {
        // Check if height has changed
        if (Math.Abs(_y - transform.position.y) > .05)
        {
            RaycastHit hitInfo;
            var hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 50);
            if (hit)
            {
                LineRenderer.SetPosition(1, transform.InverseTransformPoint(hitInfo.point));
            }

            _y = transform.position.y;
        }
    }

    #endregion

    public void SetColor(Color color)
    {
        Sprite.GetComponent<SpriteRenderer>().color = color;
        LineRenderer.startColor = LineRenderer.endColor = color;
        SelectionNumber.color = color;
    }


    public void SetSelected(bool selected, int index = -1)
    {
        if (_selected != selected)
        {
            _selected = selected;
            HoverAnimation(false);
            SelectAnimation(selected, index);
        }
        else
        {
            SelectionNumber.text = index.ToString();
        }
    }

    public void SetHover(bool hover)
    {
        if (_hover != hover)
        {
            _hover = hover;
            HoverAnimation(hover);
        }
    }

    private void HoverAnimation(bool hover)
    {
        if (hover)
        {
            _hoverSequence.PlayForward();
        }
        else
        {
            _hoverSequence.PlayBackwards();
        }
    }

    public void SelectAnimation(bool selected, int index)
    {
        _selectionSequence?.Kill();
        _selectionSequence = DOTween.Sequence();

        if (selected)
        {
            SelectionNumber.text = index.ToString();
            _selectionSequence.Append(transform.DOLocalMoveY(-2f, .1f).SetRelative(true));
            _selectionSequence.Append(transform.DOLocalMoveY(17f, .5f).SetRelative(true));
            _selectionSequence.Append(Sprite.GetComponent<SpriteRenderer>().DOFade(0f, .2f));
            _selectionSequence.Join(SelectionNumber.DOFade(1f, .2f));
        }
        else
        {
            _selectionSequence.Append(transform.DOLocalMoveY(-15f, .5f).SetRelative(true));
            _selectionSequence.Join(SelectionNumber.DOFade(0f, .1f));
            _selectionSequence.Join(Sprite.GetComponent<SpriteRenderer>().DOFade(1f, .2f));
        }
        
        _selectionSequence.Play();
    }


}
