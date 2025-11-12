using FMODUnity;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    private StudioEventEmitter emitter;

    [SerializeField] private float walkInterval = 0.6f;
    [SerializeField] private float runInterval = 0.35f;
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float runThreshold = 5f;

    public CharacterController controller;
    private float stepCooldown;

    private void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    private void Update()
    {
        float speed = controller.velocity.magnitude;

        if (speed > minSpeed)
        {
            stepCooldown -= Time.deltaTime;

            if (stepCooldown <= 0f)
            {
                emitter.Play();

                stepCooldown = speed > runThreshold ? runInterval : walkInterval;
            }
        }
        else
        {
            // Reset timer when not moving, so we don’t stack short cooldowns
            stepCooldown = 0f;
        }
    }
}
