using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase
{
    public class BaseManager<T> where T : new()
    {
        private static T m_instance;

        public static T GetInstance()
        {
            if (m_instance == null)
                m_instance = new T();

            return m_instance;
        }
    }
}
