using System.Collections.Generic;
using UnityEngine;

namespace Jinja.Scripts
{
[System.Serializable]
public class DictionaryKeyClip : Serialize.TableBase<string, AudioClip, KeyClipPair>
{
}

[System.Serializable]
public class KeyClipPair : Serialize.KeyAndValue<string, AudioClip>
{
    public KeyClipPair (string key, AudioClip value) : base (key, value)
    {

    }
}

public class JinjaSoundManager : MonoBehaviour
{
    [SerializeField]
    private DictionaryKeyClip _audioClips;
    private AudioSource _bgmSource;
    private AudioSource _effectSource;

    void Init ()
    {
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _effectSource = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private static JinjaSoundManager _instance = null;
    public static JinjaSoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var prefab = Resources.Load<GameObject>("JinjaSoundManagerPrefab");
                var obj = Instantiate(prefab);
                _instance = obj.GetComponent<JinjaSoundManager>();
                _instance.Init();
            }

            return _instance;
        }
    }

    public void PlayBgm(string key)
    {
        if (_bgmSource.isPlaying)
        {
            return;
        }

        _bgmSource.volume = 0.05f;
        _bgmSource.clip = _audioClips.GetTable()[key];
        _bgmSource.Play();
        _bgmSource.loop = true;
    }

    public void PlayOneShot(string key)
    {
        _effectSource.PlayOneShot(_audioClips.GetTable()[key]);
    }
}
}
