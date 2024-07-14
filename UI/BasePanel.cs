using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBase
{
    public class BasePanel : MonoBehaviour
    {
        private Dictionary<string, List<UIBehaviour>> m_controlDic = new Dictionary<string, List<UIBehaviour>>();

        protected virtual void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
        }

        protected virtual void OnClick(string btnName)
        {

        }

        protected virtual void OnValueChanged(string btnName, bool state)
        {

        }

        /// <summary>
        /// 从子对象上获取控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if(m_controlDic.ContainsKey(controlName))
            {
                foreach(UIBehaviour item in m_controlDic[controlName])
                {
                    if(item is T)
                        return item as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 把子对象上的控件全部记录到字典中,并且给Button和Toggle添加监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>();

            foreach (T control in controls)
            {
                string objName = control.gameObject.name;
                if(m_controlDic.ContainsKey(objName))
                {
                    m_controlDic[objName].Add(control);
                }else
                {
                    m_controlDic.Add(objName, new List<UIBehaviour>() { control });
                }

                if(control is Button)
                {
                    (control as Button).onClick.AddListener(()=>
                    {
                        OnClick(objName);
                    });
                }else if(control is Toggle)
                {
                    (control as Toggle).onValueChanged.AddListener((value)=>
                    {
                        OnValueChanged(objName, value);
                    });
                }
            }
        }


        public virtual void ShowMe()
        {

        }

        public virtual void HideMe()
        {

        }
    }
}
