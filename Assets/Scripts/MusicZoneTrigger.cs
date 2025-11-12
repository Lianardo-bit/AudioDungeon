using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    [SerializeField] private string parameterName = "MusicZone";
    [SerializeField] private float parameterValue = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.SetMusicParameter(parameterName, parameterValue);
        }
    }
}