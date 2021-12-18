using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        EventManager.TrashEmptied += PlayHappy;
        DontDestroyOnLoad(gameObject);
        sounds.ForEach(sound =>
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        });
        sounds[0].source.Play();
    }

    public void LevelUpSound()
    {
        sounds[2].source.Play();
    }

    private void PlayHappy(object sender, EventArgs args)
    {
        sounds[1].source.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // sounds[0].source.pitch = 1f - TrashMaster.TrashCapacity * 0.5f;
    }
}
