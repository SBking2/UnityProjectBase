using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProjectBase
{
    /// <summary>
    /// 定义一个key规则，keyName_数据类型_字段类型_字段名
    /// 对于List 则是 keyName_数据类型_List_字段名 表示 list.Count  其后加index表示元素
    /// 对于Dictionary keyName_数据类型_Dictionary_字段名 表示 dictionary.Count 其后 _key_index 或者 _value_index
    /// </summary>
    public class PlayerPrefsMgr : BaseManager<PlayerPrefsMgr>
    {
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="className"></param>
        /// <param name="data"></param>
        public void SaveData(string keyName, object data)
        {
            Type dataType = data.GetType();
            FieldInfo[] fieldInfos = dataType.GetFields();

            for(int i = 0; i < fieldInfos.Length; i++)
            {
                //Save 字段
                SaveValue(keyName + "_" + dataType.Name + "_" + fieldInfos[i].FieldType.Name + "_" + fieldInfos[i].Name
                    , fieldInfos[i].GetValue(data));
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// 为字段单独存值
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        private void SaveValue(string keyName, object value)
        {
            Debug.Log(value);
            Type fieldType = value.GetType();

            if(fieldType == typeof(int))
            {
                PlayerPrefs.SetInt(keyName, (int)value);
            }
            else if(fieldType == typeof(float))
            {
                PlayerPrefs.SetFloat(keyName, (float)value);
            }
            else if(fieldType == typeof(string))
            {
                PlayerPrefs.SetString(keyName, value.ToString());
            }
            else if(fieldType == typeof(bool))
            {
                PlayerPrefs.SetInt(keyName, (bool)value ? 1 : 0);
            }
            //若IList可以分配fieldType的空间，说明fieldType是IList的子类，说明是List
            //List的Key代表List的长度，后面加上index表示元素
            else if(typeof(IList).IsAssignableFrom(fieldType))
            {
                IList list = value as IList;
                //先记录list长度
                PlayerPrefs.SetInt(keyName, list.Count);

                //用Index表示第几个元素
                int index = 0;
                foreach(object obj in list)
                {
                    SaveValue(keyName + index, obj);
                    index++;
                }
            }
            else if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                IDictionary dic = value as IDictionary;
                //先记录list长度
                PlayerPrefs.SetInt(keyName, dic.Count);

                //用Index表示第几个元素
                int index = 0;
                foreach (object key in dic.Keys)
                {
                    SaveValue(keyName + "_key_" + index, key);
                    SaveValue(keyName + "_value_" + index, dic[key]);
                    index++;
                }
            }
            //其他自定义类直接递归即可
            else
            {
                SaveData(keyName, value);
            }

        }

        /// <summary>
        /// 根据自己设定的键来获取数据
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadData(string keyName, Type type)
        {
            object data = Activator.CreateInstance(type);

            //获取字段信息
            FieldInfo[] fieldInfos = type.GetFields();

            for(int i = 0; i < fieldInfos.Length;i++)
            {
                Type fieldType = fieldInfos[i].FieldType;
                string fieldName = fieldInfos[i].Name;

                string key = keyName + "_" + type.Name + "_" + fieldType.Name + "_" + fieldName;

                //获取字段值并赋值给data
                fieldInfos[i].SetValue(data, LoadValue(key, fieldType));

            }

            return data;
        }

        /// <summary>
        /// 读取单个字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object LoadValue(string key, Type type)
        {
            if(type == typeof(int))
            {
                return PlayerPrefs.GetInt(key, 0);
            }else if(type == typeof(float))
            {
                return PlayerPrefs.GetFloat(key, 0);
            }else if (type == typeof(string))
            {
                return PlayerPrefs.GetString(key, "");
            }else if(type == typeof(bool))
            {
                return PlayerPrefs.GetInt(key, 0) == 1 ? true : false;
            }
            else if(typeof(IList).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key, 0);

                IList list = Activator.CreateInstance(type) as IList;
                for(int i = 0; i < count; i++)
                {
                    list.Add(LoadValue(key + i, type.GetGenericArguments()[0]));
                }
                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                int count = PlayerPrefs.GetInt(key, 0);

                IDictionary dic = Activator.CreateInstance(type) as IDictionary;
                Type[] types = type.GetGenericArguments();
                for (int i = 0; i < count; i++)
                {
                    dic.Add(
                        LoadValue(key + "_key_" + i, types[0])
                        , LoadValue(key + "_value_" + i, types[1])
                        );
                }
                return dic;
            }else
            {
                return LoadData(key, type);
            }
        }
    }
}
