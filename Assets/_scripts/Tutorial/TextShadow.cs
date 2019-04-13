using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextShadow : MonoBehaviour
{
    public TextMesh ParentText;

    private TextMesh SelfMesh;

    // Start is called before the first frame update
    void Start()
    {
        SelfMesh = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        SelfMesh.text = ParentText.text;
    }
}
