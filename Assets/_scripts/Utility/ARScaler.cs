using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UserInterface;

[RequireComponent(typeof(ARSessionOrigin))]
public class ARScaler : Singleton<ARScaler>
{
    private ARSessionOrigin _sessionOrigin;
    public GameObject Reference;

    [Range(0f, 5f)]
    public float ScaleValue = 2.5f;
    [Range(0f, 360f)]
    public float RotationValue = 0f;

    /* Next values must be the same min and max values of
     * the Slider to change the scale */
    private float _maxScaleValue = 150f;
    private float _minScaleValue = 0.0f;
    private Quaternion _rotationQ;


    #region Unity Methods

    void Awake()
    {
        _sessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Start()
    {
        Scale(ScaleValue);
        Rotate(RotationValue);
    }

    #endregion

    // Method called by a Slider
    public void Scale(float value)
    {
        Transform t = gameObject.transform;

        _sessionOrigin.MakeContentAppearAt(
            Reference.transform,
            Reference.transform.position);

        ScaleValue = Mathf.Clamp(value, _minScaleValue, _maxScaleValue);
        t.localScale = Vector3.one * ScaleValue;
    }

    public void Rotate(float value)
    {
        Transform t = gameObject.transform;

        _sessionOrigin.MakeContentAppearAt(
            Reference.transform,
            Reference.transform.rotation);

        RotationValue = Mathf.Clamp(value, 0f, 360f);
        _rotationQ = Quaternion.AngleAxis(RotationValue, Vector3.up);
        t.rotation = _rotationQ;
    }


    public void SetRotation(float rotationY)
    {
        RotationValue = rotationY;
        UserInterfaceManager.Instance.LevelPlacementCanvas.SetRotation(rotationY);
    }
}