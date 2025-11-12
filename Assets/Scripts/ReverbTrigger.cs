using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ReverbTriggerZone : MonoBehaviour
{
        [Header("Reverb Settings")]
        [Range(0f, 100f)] public float reverbValue = 0.5f;
        [Range(0f, 100f)] public float reverbSize = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Triggered by: " + other.name);
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player Triggered Reverb Zone");
                EventInstance speechEvent = RuntimeManager.CreateInstance("event:/SayANumber");
                EventInstance stepsEvent = RuntimeManager.CreateInstance("event:/Footsteps");
                
                speechEvent.setParameterByName("ReverbValue", reverbValue);
                stepsEvent.setParameterByName("ReverbValue", reverbValue*2);
                
                RuntimeManager.StudioSystem.setParameterByName("ReverbSize", reverbSize);
            }
        }
}