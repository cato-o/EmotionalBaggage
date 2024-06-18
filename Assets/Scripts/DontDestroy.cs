using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persistentObjects = new GameObject[1];
    [SerializeField]
    private int objectIndex;

    void Awake()
    {
        if(persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        
        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
