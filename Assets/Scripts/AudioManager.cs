using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AudioType
{
    Tile,
    Line,
    UnTile,
}
public class AudioManager : MonoBehaviour
{

    [SerializeField] private List<AudioClip> tileClips;
    [SerializeField] private List<AudioClip> lineClips;

    private static Queue<AudioType> audioQueue = new Queue<AudioType>();
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (audioQueue.Count > 0)
        {
            while (audioQueue.TryDequeue(out var audioType))
            {
                AudioClip clip;
                float volume = 0.5f;
                switch (audioType)
                {
                    case AudioType.Tile:
                        clip = tileClips[Random.Range(0, tileClips.Count)];
                        break;
                    case AudioType.Line:
                        clip = lineClips[Random.Range(0, lineClips.Count)];
                        break;
                    case AudioType.UnTile:
                        clip = lineClips[Random.Range(0, lineClips.Count)];
                        volume = 0.2f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                source.PlayOneShot(clip, volume);

            }
        }        
    }

    public static void queueSound(AudioType type)
    {
        audioQueue.Enqueue(type);
    }
}
