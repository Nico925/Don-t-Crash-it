using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AudioMng : MonoBehaviour
{
    public AudioClip InputClick;
    public AudioClip InputHover;
    public AudioClip Altimeter;
    public AudioClip Alarm;
    public AudioClip MenuInput;
    public AudioClip PuzzleSolved;
    public List<AudioClip> Ambient = new List<AudioClip>();
    public List<AudioClip> AmbientTransitions = new List<AudioClip>();
    public float FadeTime = 0.5f;

    private List<AudioSource> audioSources = new List<AudioSource>();
    #region API
    /// <summary>
    /// Play a specific AudioType.
    /// Throws error if no Clip is assigned to the specific type.
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_loop"></param>
    public void PlaySound(AudioType _type, bool _loop = false) 
    {
        AudioSource sourceToUse = GetFirstSourceByType(_type);

        switch (_type)
        {
            case AudioType.InputClick:
                sourceToUse.clip = InputClick;
                break;
            case AudioType.InputHover:
                sourceToUse.clip = InputHover;
                break;
            case AudioType.Ambient:
                PlayAmbient();
                break;
            case AudioType.Altimeter:
                sourceToUse.clip = Altimeter;
                break;
            case AudioType.Alarm:
                sourceToUse.clip = Alarm;
                break;
            case AudioType.MenuInput:
                sourceToUse.clip = MenuInput;
                break;
            case AudioType.PuzzleSolved:
                sourceToUse.clip = PuzzleSolved;
                break;
            default:
                break;
        }
        //Fade In for Ambient
        if (_type == AudioType.Ambient)
            sourceToUse.volume = 0;

        //General clip start
        sourceToUse.Play();

        //Fade In for Ambient
        if (sourceToUse.volume == 0)
            sourceToUse.DOFade(1, FadeTime);

        if (_loop)
            sourceToUse.loop = true;
    }
    /// <summary>
    /// Play a specific AudioType.
    /// Throws error if no Clip is assigned to the specific type.
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_loop"></param>
    public void PlaySound(AudioType _type, bool _overrideCurrent, bool _loop = false)
    {
        AudioSource sourceToUse = GetFirstSourceByType(_type);

        switch (_type)
        {
            case AudioType.InputClick:
                sourceToUse.clip = InputClick;
                break;
            case AudioType.InputHover:
                sourceToUse.clip = InputHover;
                break;
            case AudioType.Ambient:
                PlayAmbient();
                break;
            case AudioType.Altimeter:
                sourceToUse.clip = Altimeter;
                break;
            case AudioType.Alarm:
                sourceToUse.clip = Alarm;
                break;
            case AudioType.MenuInput:
                sourceToUse.clip = MenuInput;
                break;
            case AudioType.PuzzleSolved:
                sourceToUse.clip = PuzzleSolved;
                break;
            default:
                break;
        }

        //Fade In for Ambient
        if (_type == AudioType.Ambient)
            sourceToUse.volume = 0;

        //General clip start
        sourceToUse.Play();

        //Fade In for Ambient
        if (sourceToUse.volume == 0)
            sourceToUse.DOFade(1, FadeTime);

        if (_loop)
            sourceToUse.loop = true;
    }
    /// <summary>
    /// Play a specific Clip.
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_loop"></param>
    public void PlaySound(AudioClip _clip, bool _loop = false)
    {
        AudioSource sourceToUse = GetFirstAvaibleSource();

        sourceToUse.clip = _clip;

        //sourceToUse.volume = 0;
        sourceToUse.Play();
        //sourceToUse.DOFade(1, FadeTime);
        if (_loop)
            sourceToUse.loop = true;
    }
    /// <summary>
    /// Shout down all the Audio Sources
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            Destroy(audioSources[i].gameObject);
        }
        for (int i = 0; i < ambientSources.Count; i++)
        {
            if(ambientSources[i] != null)
                Destroy(ambientSources[i].gameObject);
        }

        ambientSources.Clear();
        audioSources.Clear();
        StopAllCoroutines();
    }
    /// <summary>
    /// Fade all the Audio Source to _endValue
    /// </summary>
    /// <param name="_endValue"></param>
    public void FadeAll(int _endValue)
    {
        foreach (AudioSource source in audioSources)
        {
            source.DOFade(_endValue, 0.1f);
        }
    }

    int ambientIndex = 0;
    List<AudioSource> ambientSources = new List<AudioSource>();
    public void PlayAmbient(int _specific = -1)
    {
        AudioSource newSource = GetNewSource();

        foreach (AudioSource source in ambientSources)
        {
            if(source != null)
                source.DOFade(0, FadeTime).OnComplete(()=> { Destroy(source.gameObject); });
            audioSources.Remove(source);
        }

        ambientSources.Add(newSource);

        newSource.volume = 0;
        newSource.loop = true;

        if (_specific >= 0 && _specific < Ambient.Count)
        {
            ambientIndex = _specific;
            newSource.clip = Ambient[ambientIndex];
        }
        else
        {
            if (ambientIndex == 0)
            {
                PlayAmbient(0);
                return;
            }
            else if (ambientIndex == 1)
            {
                PlayAmbient(1);
                return;
            }
            else if (ambientIndex == 2)
            {
                newSource.clip = AmbientTransitions[0];
                StartCoroutine(PlayAmbientPostTransition(0));
                return;
            }
            else if (ambientIndex == 3)
            {
                newSource.clip = AmbientTransitions[1];
                StartCoroutine(PlayAmbientPostTransition(1));
                return;
            }
            else if (ambientIndex == 4)
            {
                newSource.clip = AmbientTransitions[2];
                StartCoroutine(PlayAmbientPostTransition(2));
                return;
            }
        }

        newSource.DOFade(1, FadeTime);
        newSource.Play();
    }

    public void PlayClipOnce(AudioClip _clip)
    {
        PlaySound(_clip, false);
    }
    #endregion

    IEnumerator PlayAmbientPostTransition(int _transitionID)
    {
        yield return new WaitForSeconds(AmbientTransitions[_transitionID].length);
        PlayAmbient(_transitionID + 2);
    }
    /// <summary>
    /// Create a new source
    /// </summary>
    /// <returns></returns>
    AudioSource GetNewSource()
    {
        AudioSource newSource = null;

        newSource = Instantiate(new GameObject("Source_" + audioSources.Count), transform).AddComponent<AudioSource>();
        audioSources.Add(newSource);

        return newSource;
    }
    /// <summary>
    /// Return the first Avaiable AudioSource.
    /// It creates a new one if none foun.
    /// </summary>
    /// <returns></returns>
    AudioSource GetFirstAvaibleSource()
    {
        AudioSource firstFreeSource = null;
        //Search for thee first avaiable free AudioSource
        foreach (var audio in audioSources)
        {
            if (audio.isPlaying == false)
            {
                firstFreeSource = audio;
                break;
            }
        }
        //If none found it creates a new one;
        if (firstFreeSource == null)
        {
            firstFreeSource = GetNewSource();
        }

        return firstFreeSource;
    }
    /// <summary>
    /// Return the first AudioSource that's playing a specific AudioType
    /// It creates a new one if none found.
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    AudioSource GetFirstSourceByType(AudioType _type, bool _returnNull = false)
    {
        AudioSource sourceByType = null;

        AudioClip referenceClip = null;
        switch (_type)
        {
            case AudioType.InputClick:
                referenceClip = InputClick;
                break;
            case AudioType.InputHover:
                referenceClip = InputHover;
                break;
            case AudioType.Altimeter:
                referenceClip = Altimeter;
                break;
            case AudioType.Alarm:
                referenceClip = Alarm;
                break;
            case AudioType.PuzzleSolved:
                referenceClip = PuzzleSolved;
                break;
            default:
                break;
        }

        foreach (AudioSource source in audioSources)
        {
            if (source.clip == referenceClip)
            {
                sourceByType = source;
                break;
            }

            if(_type == AudioType.Ambient)
                if (Ambient.Contains(source.clip) || AmbientTransitions.Contains(source.clip))
                {
                    sourceByType = source;
                    break;
                }
        }

        if(!_returnNull)
            if(sourceByType == null)
                sourceByType = GetFirstAvaibleSource();

        return sourceByType;
    }
}

public enum AudioType 
{
    InputClick = 0,
    InputHover,
    Ambient,
    Altimeter,
    Alarm,
    MenuInput,
    PuzzleSolved
}

