using FMODUnity;
using UnityEngine;

public class SayANumber : MonoBehaviour
{
    private StudioEventEmitter emitter;

    void Start()
    {
        // Get the FMOD Studio Event Emitter on this GameObject
        emitter = GetComponent<StudioEventEmitter>();
    }

    void Update()
    {
        // When player presses E, play the FMOD event
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (emitter != null)
            {
                emitter.Play();
            }
            else
            {
                Debug.LogWarning("No FMOD Studio Event Emitter found on this GameObject.");
            }
        }
    }
}
