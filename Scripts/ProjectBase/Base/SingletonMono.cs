using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;

        /// <summary>
        /// Instance只会指向最后Awake的脚本，所以在U3D中只能挂载一个
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            return m_instance;
        }

        protected virtual void Awake()
        {
            m_instance = this as T;
        }
    }
}
