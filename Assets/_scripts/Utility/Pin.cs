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

    private bool _selected;
    private Camera _camera;
    private float _y;
    private Sequence _sequence;

    #region Unity Methods

    private void Awake()
    {
        _camera = Camera.main;
        SelectionNumber.DOFade(0, 0);
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
            SelectAnimation(selected, index);
        }
        else
        {
            SelectionNumber.text = index.ToString();
        }
    }

    public void SelectAnimation(bool selected, int index)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();


        if (selected)
        {
            SelectionNumber.text = index.ToString();
            _sequence.Append(transform.DOMoveY(-2f, .1f).SetRelative(true));
            _sequence.Append(transform.DOMoveY(17f, .5f).SetRelative(true));
            _sequence.Append(Sprite.GetComponent<SpriteRenderer>().DOFade(0f, .2f));
            _sequence.Join(SelectionNumber.DOFade(1f, .2f));
        }
        else
        {
            _sequence.Append(transform.DOMoveY(-15f, .5f).SetRelative(true));
            _sequence.Join(SelectionNumber.DOFade(0f, .1f));
            _sequence.Join(Sprite.GetComponent<SpriteRenderer>().DOFade(1f, .2f));
        }
        
        _sequence.Play();
    }


}
