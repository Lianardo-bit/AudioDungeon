using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private EventReference musicEvent;
    private EventInstance musicInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartMusic();
    }

    private void StartMusic()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    public void SetMusicParameter(string parameterName, float value)
    {
        musicInstance.setParameterByName(parameterName, value);
    }

    private void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}