using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotionalBaggage;



public class AudioManager : MonoBehaviour
{
    private static AudioManager audioManagerInstance;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if(audioManagerInstance == null)
        {
            audioManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
