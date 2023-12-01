using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    private class AudioData
    {
        public string name;
        public AudioClip clip;
        public float volume;
        public bool isLoop;
        public AudioSource[] source;
    }

    [SerializeField] private List<AudioData> audioDatas;

    public static Action<string, bool, bool> PlayAudio;
    public static Action<string> StopAudio;

    #region SingleTon
    public static SoundManager Instance;
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void OnEnable()
    {
        PlayAudio += OnPlayAudio;
        StopAudio += OnStopAudio;
    }

    private void OnDisable()
    {
        PlayAudio -= OnPlayAudio;
        StopAudio -= OnStopAudio;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayAudio(string name, bool changePitch, bool overridePreviousPlayingAudio)
    {
        for (int i = 0; i < audioDatas.Count; i++)
        {
            if (audioDatas[i].name == name)
            {
                Debug.Log($"Audio Playing {name}");

                for (int j = 0; j < audioDatas[i].source.Length; j++)
                {
                    if (!audioDatas[i].source[j].isPlaying)
                    {
                        audioDatas[i].source[j].clip = audioDatas[i].clip;
                        audioDatas[i].source[j].volume = audioDatas[i].volume;
                        audioDatas[i].source[j].loop = audioDatas[i].isLoop;

                        if(changePitch)
                        {
                            audioDatas[i].source[j].pitch = Random.Range(0.75f, 1.15f);
                        }

                        audioDatas[i].source[j].Play();

                        if (!audioDatas[i].isLoop)
                        {
                            StartCoroutine(StopAudioWithDelay(audioDatas[i].clip.length, i, j));
                        }
                        break;
                    }

                    if (audioDatas[i].source[j].isPlaying && j == audioDatas[i].source.Length - 1 && overridePreviousPlayingAudio)
                    {
                        audioDatas[i].source[0].Stop();

                        audioDatas[i].source[0].clip = audioDatas[i].clip;
                        audioDatas[i].source[0].volume = audioDatas[i].volume;
                        audioDatas[i].source[0].loop = audioDatas[i].isLoop;

                        if (changePitch)
                        {
                            audioDatas[i].source[j].pitch = Random.Range(0.75f, 1.15f);
                        }

                        audioDatas[i].source[0].Play();

                        if (!audioDatas[i].isLoop)
                        {
                            StartCoroutine(StopAudioWithDelay(audioDatas[i].clip.length, i, j));
                        }
                        break;
                    }
                }
            }
        }
    }

    IEnumerator StopAudioWithDelay(float delay, int audioDataIndex, int audioSourceIndex)
    {
        yield return new WaitForSeconds(delay + 0.1f);

        Debug.Log($"Audio Stopping {audioDataIndex} {audioSourceIndex}");

        audioDatas[audioDataIndex].source[audioSourceIndex].Stop();
    }

    public void OnStopAudio(string name)
    {
        for (int i = 0; i < audioDatas.Count; i++)
        {
            if (audioDatas[i].name == name)
            {
                Debug.Log($"Audio Stopping {name}");

                for (int j = 0; j < audioDatas[i].source.Length; j++)
                {
                    if (audioDatas[i].source[j].isPlaying)
                    {
                        audioDatas[i].source[j].Stop();
                    }
                }
            }
        }
    }
}
