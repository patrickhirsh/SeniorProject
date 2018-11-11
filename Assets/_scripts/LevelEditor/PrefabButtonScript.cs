using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabButtonScript : MonoBehaviour {

    public Canvas pMenu;

    public void switchMenu()
    {
        if (pMenu.gameObject.activeSelf)
        {
            pMenu.gameObject.SetActive(false);
        }
        else
        {
            pMenu.gameObject.SetActive(true);
        }
    }
}
