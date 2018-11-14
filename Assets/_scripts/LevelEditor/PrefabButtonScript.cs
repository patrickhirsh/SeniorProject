using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabButtonScript : MonoBehaviour {

    public Canvas PMenu;

    public void SwitchMenu()
    {
        if (PMenu.gameObject.activeSelf)
        {
            PMenu.gameObject.SetActive(false);
        }
        else
        {
            PMenu.gameObject.SetActive(true);
        }
    }
}
