using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;


public class PassengerPin : LevelObject
{
    public Passenger Passenger;

    public CanvasGroup CanvasGroup;
    public Image Icon;
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
        InputManager.Instance.HoverChanged.AddListener(HandleHoverChange);

        _hoverSequence = DOTween.Sequence();
        _hoverSequence.Append(transform.DOLocalMoveY(Passenger.HoverHeight, .2f));
        _hoverSequence.SetAutoKill(false).Pause();
    }

    public void SetPassenger(Passenger passenger)
    {
        Passenger = passenger;
    }

    private void HandleHoverChange(GameObject go)
    {
        var isHovering = go == gameObject && !_selected;
        SetHover(isHovering);
    }

    private void Update()
    {
        if (CanvasGroup.gameObject.activeInHierarchy)
        {
            CanvasGroup.transform.LookAt(_camera.transform);
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
        Icon.color = color;
        SelectionNumber.color = color;
        RadialTimerImg.color = color;
        LineRenderer.startColor = LineRenderer.endColor = color;
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
            _selectionSequence.Append(transform.DOLocalMoveY(-3f, .1f).SetRelative(true));
            _selectionSequence.Append(transform.DOLocalMoveY(Passenger.SelectedHeight, .5f));
            _selectionSequence.Append(Icon.DOFade(0f, .2f));
            _selectionSequence.Join(SelectionNumber.DOFade(1f, .2f));
        }
        else
        {
            _selectionSequence.Append(transform.DOLocalMoveY(Passenger.DefaultHeight, .5f));
            _selectionSequence.Join(SelectionNumber.DOFade(0f, .1f));
            _selectionSequence.Join(Icon.DOFade(1f, .2f));
        }
        
        _selectionSequence.Play();
    }



}
