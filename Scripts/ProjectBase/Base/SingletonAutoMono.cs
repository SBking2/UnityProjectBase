using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase
{
    public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;

        public static T GetInstance()
        {
            if(m_instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                //让系统不要删除这个obj
                DontDestroyOnLoad(obj);
                m_instance  = obj.AddComponent<T>();
            }

            return m_instance;
        }
    }
}
