using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("Discription"))]
public class DiscriptionData : ScriptableObject
{
    public string[] MainName;
    public string[] SubName;
    [TextArea(3,5)] public string[] MainDiscription;
    [TextArea(3,5)] public string[] SubDiscription;
}
