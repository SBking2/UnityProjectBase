using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase
{
    public class InputMgr : BaseManager<InputMgr>
    {
        private bool m_isStartCheckKey = false;

        public InputMgr() 
        {
            MonoMgr.GetInstance().AddUpdateListener(Update);
        }

        private void Update()
        {
            if (m_isStartCheckKey == false)
                return;

            //TODO：添加InputMgr要监听的输入
        }

        public void StartOrEndCheckKey(bool value)
        {
            m_isStartCheckKey = value;
        }

        private void CheckKey(KeyCode keyCode)
        {
            if(Input.GetKeyDown(keyCode))
            {
                EventCenter.GetInstance().EventTrigger<KeyCode>("SomeKeyDown", keyCode);
            }

            if(Input.GetKeyUp(keyCode))
            {
                EventCenter.GetInstance().EventTrigger<KeyCode>("SomeKeyUp", keyCode);
            }

            if(Input.GetKey(keyCode))
            {
                EventCenter.GetInstance().EventTrigger<KeyCode>("SomeKey", keyCode);
            }
        }
    }
}
