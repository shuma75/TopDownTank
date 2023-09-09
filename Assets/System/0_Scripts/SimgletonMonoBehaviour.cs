using System.Collections;
using System.Collections.Generic;
using Taki.TakiAESJsonSave;
using UnityEngine;

public class SimgletonMonoBehaviour<T>:MonoBehaviour where T: class, new() 
{
    public static T Instance;

    protected virtual void Awake()
    {
        if(Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
