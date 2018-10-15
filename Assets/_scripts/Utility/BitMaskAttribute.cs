using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Have to be defined somewhere in a runtime script file
public class BitMaskAttribute : PropertyAttribute
{
    public System.Type PropType;
    public BitMaskAttribute(System.Type aType)
    {
        PropType = aType;
    }
}
