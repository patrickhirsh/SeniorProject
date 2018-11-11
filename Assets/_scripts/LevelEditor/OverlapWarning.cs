using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapWarning : MonoBehaviour {

    public LayerMask m_LayerMask;
    public bool m_Started;
    public bool Overlapping;
    public LevelEditorController LEC;

    void Start()
    {
        m_Started = true;
        LEC = GameObject.Find("[LevelEditorManager]").GetComponent<LevelEditorController>();
    }

    void FixedUpdate()
    {
        MyCollisions();
    }

    void MyCollisions()
    {

        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position,transform.localScale/2, Quaternion.identity, m_LayerMask);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        if (i < hitColliders.Length - 1 )
        {
           if(hitColliders[i].name != this.name && Overlapping == false)
            {
                //Output all of the collider names
                Debug.Log(this.name + "   Hit : " + hitColliders[i].name);
                Overlapping = true;
                LEC.Overlapping++;
                return;
            }
            
            i++;
        }
        else if(Overlapping == true)
        {
            Overlapping = false;
            LEC.Overlapping--;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (m_Started)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Warning: Overlapping tiles");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Moved away");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ENGERED");
    }
}
