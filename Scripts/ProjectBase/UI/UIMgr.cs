using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ProjectBase
{
    public enum UILayer
    {
        Top,
        Bottom,
        Mid,
        System
    }
    public class UIMgr : BaseManager<UIMgr>
    {

        Dictionary<string, BasePanel> m_panelDic = new Dictionary<string, BasePanel>();

        private Transform m_top;
        private Transform m_bottom;
        private Transform m_mid;
        private Transform m_system;

        public RectTransform canvas;

        public UIMgr()
        {
            GameObject obj = ResMgr.GetInstance().Load<GameObject>("UI/Canvas");
            canvas = obj.transform as RectTransform;
            GameObject.DontDestroyOnLoad(obj);

            m_top = canvas.Find("top");
            m_bottom = canvas.Find("bottom");
            m_mid = canvas.Find("mid");
            m_system = canvas.Find("system");

            obj = ResMgr.GetInstance().Load<GameObject>("UI/EventSystem");
            GameObject.DontDestroyOnLoad(obj);
        }

        /// <summary>
        /// 显示面板函数
        /// </summary>
        /// <typeparam name="T">面板携带的脚本类型</typeparam>
        /// <param name="panelName"></param>
        /// <param name="callback">读取面板结束后执行的函数</param>
        /// <param name="layer"></param>
        public void ShowPanel<T>(string panelName, UILayer layer = UILayer.Mid, UnityAction<T> callback = null) where T : BasePanel
        {
            if(m_panelDic.ContainsKey(panelName))
            {
                m_panelDic[panelName].ShowMe();
                if (callback != null)
                    callback(m_panelDic[panelName] as T);

                return;
            }

            ResMgr.GetInstance().LoadAsny<GameObject>(panelName, (obj)=>
            {
                Transform father = m_mid;
                switch (layer)
                {
                    case UILayer.Mid:
                        father = m_mid;
                        break;
                    case UILayer.Top:
                        father = m_top;
                        break;
                    case UILayer.Bottom:
                        father = m_bottom;
                        break;
                    case UILayer.System:
                        father = m_system;
                        break;
                }

                obj.transform.SetParent(father);

                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;

                (obj.transform as RectTransform).offsetMax = Vector2.zero;
                (obj.transform as RectTransform).offsetMin = Vector2.zero;

                T panelScript = obj.GetComponent<T>();

                if(callback != null)
                {
                    callback(panelScript);
                }

                panelScript.ShowMe();

                m_panelDic.Add(panelName, panelScript);
            });
        }


        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <param name="panelName"></param>
        public void HidePanel(string panelName)
        {
            if(m_panelDic.ContainsKey(panelName))
            {
                m_panelDic[panelName].HideMe();
                GameObject.Destroy(m_panelDic[panelName].gameObject);
                m_panelDic.Remove(panelName);
            }
        }

        public T GetPanel<T>(string name) where T : BasePanel
        {
            if (m_panelDic.ContainsKey(name))
            {
                return m_panelDic[name] as T;
            }
            return null;
        }

        /// <summary>
        /// 添加自定义监听事件
        /// </summary>
        /// <param name="control">要添加监听的控件</param>
        /// <param name="type">监听的事件类型</param>
        /// <param name="callback">执行的函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = control.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callback);

            trigger.triggers.Add(entry);
        }
    }
}
