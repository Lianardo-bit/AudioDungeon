using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class BombController : MonoBehaviour {
    [SerializeField] private EventReference bombSound;     // Link to FMOD bomb event
    [SerializeField] private EventReference bombSnapshot;  // Link to FMOD snapshot

    public void TriggerBomb() {
        // Play bomb sound
        RuntimeManager.PlayOneShot(bombSound);

        // Start snapshot
        EventInstance snapshot = RuntimeManager.CreateInstance(bombSnapshot);
        snapshot.start();
        snapshot.release();

        // Show speech bubble
        ShowSpeechBubble();
    }

    void ShowSpeechBubble() {
        // Instantiate prefab with your sprite
        GameObject bubble = Instantiate(Resources.Load("SpeechBubblePrefab")) as GameObject;
        bubble.transform.position = transform.position + Vector3.up * 2;

        // Destroy after 2 seconds
        Destroy(bubble, 2f);
    }
}