using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ProjectBase
{
    public class SceneMgr : BaseManager<SceneMgr>
    {
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadScene(string sceneName, UnityAction action)
        {
            SceneManager.LoadScene(sceneName);

            action();
        }

        /// <summary>
        /// 异步加载场景，开启协程
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public void LoadSceneAsyn(string sceneName, UnityAction action)
        {
            MonoMgr.GetInstance().StartRoutine(ReallyLoadSceneAsyn(sceneName, action));
        }

        private IEnumerator ReallyLoadSceneAsyn(string sceneName, UnityAction action)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

            while(!ao.isDone)
            {
                //TODO:传送给事件中心场景的加载进度
                yield return ao.progress;
            }

            action();
        }
    }
}
