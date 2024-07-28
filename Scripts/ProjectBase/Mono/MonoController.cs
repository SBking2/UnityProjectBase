using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

namespace ProjectBase
{
    public class MonoController : MonoBehaviour
    {

        private UnityAction m_updateEvent;

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            if(m_updateEvent != null)
            {
                m_updateEvent();
            }
        }

        public void AddUpdateListener(UnityAction action)
        {
            m_updateEvent += action;
        }

        public void RemoveUpdateListener(UnityAction action)
        {
            m_updateEvent -= action;
        }

        public Coroutine StartRoutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public Coroutine StartRoutine(string methodName, [DefaultValue("null")] object value)
        {
            return StartCoroutine(methodName, value);
        }

        public Coroutine StartRoutine(string methodName)
        {
            return StartCoroutine(methodName);
        }
    }
}
