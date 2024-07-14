using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectBase
{
    public class MusicMgr : BaseManager<MusicMgr>
    {
        private AudioSource m_bkMusic;
        private float m_bkVolume = 1.0f;

        private GameObject m_soundObj = null;
        private float m_soundVolume = 1.0f;
        private List<AudioSource> m_soundList = new List<AudioSource>();

        public MusicMgr()
        {
            MonoMgr.GetInstance().AddUpdateListener(Update);
        }

        private void Update()
        {
            for (int i = m_soundList.Count - 1; i >= 0; --i)
            {
                if (!m_soundList[i].isPlaying)
                {
                    GameObject.Destroy(m_soundList[i]);
                    m_soundList.RemoveAt(i);
                }
            }
        }

        public void PlayBkMusic(string name)
        {
            if(m_bkMusic == null)
            {
                GameObject obj = new GameObject("BkMusic");
                m_bkMusic = obj.AddComponent<AudioSource>();
            }

            ResMgr.GetInstance().LoadAsny<AudioClip>(name, (clip) =>
            {
                m_bkMusic.clip = clip;
                m_bkMusic.volume = m_bkVolume;
                m_bkMusic.loop = true;
                m_bkMusic.Play();
            });
        }

        public void SetBkMusicVolume(float volume)
        {
            m_bkVolume = volume;
            m_bkMusic.volume = m_bkVolume;
        }

        public void PauseBkMusic()
        {
            if(m_bkMusic != null)
            {
                m_bkMusic.Pause();
            }
        }
        public void StopBkMusic()
        {
            if(m_bkMusic != null)
            {
                m_bkMusic.Stop();
            }
        }

        public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callback = null)
        {
            if (m_soundObj == null)
            {
                m_soundObj = new GameObject("Sound");
            }

            ResMgr.GetInstance().LoadAsny<AudioClip>(name, (clip) =>
            {
                AudioSource source = m_soundObj.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = isLoop;
                source.volume = m_soundVolume;
                source.Play();
                m_soundList.Add(source);
                if(callback != null)
                    callback(source);
            });
        }

        public void SetSoundVolume(float volume)
        {
            m_soundVolume = volume;
            foreach(AudioSource source in m_soundList)
            {
                source.volume = m_soundVolume;
            }
        }

        public void StopSound(AudioSource source)
        {
            if(m_soundList.Contains(source))
            {
                m_soundList.Remove(source);
                source.Stop();
                GameObject.Destroy(source);
            }
        }
    }
}
