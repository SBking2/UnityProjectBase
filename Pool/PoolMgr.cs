using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectBase
{
    public class PoolData
    {
        public GameObject fatherObj;
        public List<GameObject> poolList;
        public PoolData(GameObject obj, GameObject poolObj)
        {
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.parent = poolObj.transform;

            poolList = new List<GameObject>();
            PushObj(obj);
        }

        public GameObject GetObj()
        {
            GameObject obj = null;
            obj = poolList[0];
            poolList.RemoveAt(0);

            obj.SetActive(true);
            obj.transform.parent = null;
            return obj;
        }
        public void PushObj(GameObject obj)
        {

            obj.SetActive(false);

            poolList.Add(obj);

            obj.transform.parent = fatherObj.transform;
        }
    }

    public class PoolMgr : BaseManager<PoolMgr>
    {
        /// <summary>
        /// 缓存池是单例模式，根据名字找不同物体
        /// </summary>
        Dictionary<string, PoolData> m_poolDic = new Dictionary<string, PoolData>();


        GameObject m_poolObj;

        public void GetObj(string name, UnityAction<GameObject> callback)
        {
            if(m_poolDic.ContainsKey(name)&& m_poolDic[name].poolList.Count > 0)
            {
                callback(m_poolDic[name].GetObj());
            }else
            {
                ResMgr.GetInstance().LoadAsny<GameObject>(name, (obj) =>
                {
                    obj.name = name;
                    callback(obj);
                });
            }
        }

        public void PushObj(string name, GameObject obj)
        {
            if (m_poolObj == null)
                m_poolObj = new GameObject("Pool");

            if(m_poolDic.ContainsKey(name))
            {
                m_poolDic[name].PushObj(obj);
            }else
            {
                m_poolDic.Add(name, new PoolData(obj, m_poolObj));
            }
        }

        public void Clear()
        {
            m_poolDic.Clear();
            m_poolObj = null;
        }
    }
}
