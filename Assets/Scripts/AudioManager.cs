using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private Sound[] sounds;

    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }else if(_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach(Sound sound in sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.loop = sound.Loop;
        }
    }

    private void Start()
    {
        this.Play("Theme");
    }

    public void Play(string filename)
    {
        Sound sound = Array.Find(sounds, s => s.Name == filename);

        if (sound == null)
            return;

        sound.Source.Play();
    }

    [System.Serializable]
    private class Sound
    {
        [SerializeField] private string name;

        [SerializeField] private AudioClip clip;

        [SerializeField] [Range(0F, 1F)]  private float volume;

        [SerializeField] private AudioSource source;

        [SerializeField] private bool loop;
       

        public string Name { get { return name; } set { name = value; } }
        public AudioClip Clip { get { return clip; } set { clip = value; } }
        public float Volume { get { return volume; } set { volume = value; } }
        public AudioSource Source { get { return source; } set { source = value; } }
        public bool Loop { get { return loop; } set { loop = value; } }
    
    }
}
