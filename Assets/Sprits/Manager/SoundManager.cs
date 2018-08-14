using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundManager : MonoBehaviour {
    private AudioSource audio;
    private Hashtable sounds = new Hashtable();
    string backSoundKey = "";

    void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio == null)
            gameObject.AddComponent<AudioSource>();
    }

    //�ص�����ԭ��
    private delegate void GetBack(AudioClip clip, string key);

    //��ȡ������Դ
    private void Get(string abName, string assetName, GetBack cb)
    {
        string key = abName + "." + assetName;
        if (sounds[key] == null)
        {
            ResourceManager.instence.LoadAudioClip(abName, assetName, (objs) =>
            {
                if (objs == null || objs[0] == null)
                {
    
                    cb(null, key);
                    return;
                }
                else
                {
                    sounds.Add(key, objs[0]);
                    cb(objs[0] as AudioClip, key);
                    return;
                }
            });
        }
        else
        {
            cb(sounds[key] as AudioClip, key);
            return;
        }
    }
    //���ű�������
    public void PlayBackSound(string abName, string assetName)
    {
        backSoundKey = abName + "." + assetName;
        Get(abName, assetName, (clip, key) =>
        {
            if (clip == null)
                return;
            if (key != backSoundKey)
                return;

            audio.loop = true;
            audio.clip = clip;
            audio.Play();
        });
    }

    //ֹͣ��������
    public void StopBackSound()
    {
        backSoundKey = "";
        audio.Stop();
    }

    //������Ч
    public void PlaySound(string abName, string assetName)
    {
        Get(abName, assetName, (clip, key) =>
        {

            if (clip == null)
                return;
            if (Camera.main == null)
                return;
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        });
    }
//===============================================================================

    /// <summary>
    /// ���һ������
    /// </summary>
    void Add(string key, AudioClip value) {
        if (sounds[key] != null || value == null) return;
        sounds.Add(key, value);
    }

    /// <summary>
    /// ��ȡһ������
    /// </summary>
    AudioClip Get(string key) {
        if (sounds[key] == null) return null;
        return sounds[key] as AudioClip;
    }

    /// <summary>
    /// ����һ����Ƶ
    /// </summary>
    public AudioClip LoadAudioClip(string path) {
        AudioClip ac = Get(path);
        if (ac == null) {
            ac = (AudioClip)Resources.Load(path, typeof(AudioClip));
            Add(path, ac);
        }
        return ac;
    }

    /// <summary>
    /// �Ƿ񲥷ű������֣�Ĭ����1������
    /// </summary>
    /// <returns></returns>
    public bool CanPlayBackSound() {
        string key = AppConst.AppPrefix + "BackSound";
        int i = PlayerPrefs.GetInt(key, 1);
        return i == 1;
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="canPlay"></param>
    public void PlayBacksound(string name, bool canPlay) {
        if (audio.clip != null) {
            if (name.IndexOf(audio.clip.name) > -1) {
                if (!canPlay) {
                    audio.Stop();
                    audio.clip = null;
                    //Util.ClearMemory();
                }
                return;
            }
        }
        if (canPlay) {
            audio.loop = true;
            audio.clip = LoadAudioClip(name);
            audio.Play();
        } else {
            audio.Stop();
            audio.clip = null;
            //Util.ClearMemory();
        }
    }

    /// <summary>
    /// �Ƿ񲥷���Ч,Ĭ����1������
    /// </summary>
    /// <returns></returns>
    public bool CanPlaySoundEffect() {
        string key = AppConst.AppPrefix + "SoundEffect";
        int i = PlayerPrefs.GetInt(key, 1);
        return i == 1;
    }

    /// <summary>
    /// ������Ƶ����
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    public void Play(AudioClip clip, Vector3 position) {
        if (!CanPlaySoundEffect()) return;
        AudioSource.PlayClipAtPoint(clip, position); 
    }
}
