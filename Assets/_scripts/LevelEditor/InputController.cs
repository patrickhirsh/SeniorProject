using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class InputController : MonoBehaviour
{
    private bool _objectSelected;

    //Buttons
    public Button PrefabButton;

    private GameObject _selected;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        HandleInput();
        if (_objectSelected)
        {
            var mousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.x);
            mousePosition.y = 0;
            var objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _selected.transform.position = objPosition;
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click");
            RaycastHit hitInfo;
            var rc = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            Debug.Log(rc);
            if (rc)
            {
                Debug.Log("Hit Object");
                _selected = hitInfo.transform.gameObject;
                _objectSelected = true;
            }
        }
    }
}