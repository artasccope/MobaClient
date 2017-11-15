using GameFW.Core.Base;
using GameFW.Core.Msg;
using GameFW.OrganizeData.Entity;
using GameFW.UI;
using GameFW.Ultility;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameFW.Asset.Load
{
    /// <summary>
    /// 场景的加载器，根据场景记载的资源加载文件
    /// </summary>
    public class SceneLoader : ResLoader
    {
        #region 单例
        private static SceneLoader instance;
        private static System.Object obj = new System.Object();
        public static SceneLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            GameObject o = GameObject.Find("mgrGameObject");
                            if (o == null)
                                o = new GameObject("mgrGameObject");
                            instance = o.AddComponent<SceneLoader>();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        /// <summary>
        /// 记录对应的资源是否加载完成了
        /// </summary>
        Dictionary<string, bool> loadedObjRecord = new Dictionary<string, bool>();
        /// <summary>
        /// 加载完成后缓存的资源,包括prefab和organize data
        /// </summary>
        Dictionary<string, Object> cacheObjs = new Dictionary<string, Object>();
        /// <summary>
        /// 需要实例化的prefab资源表
        /// </summary>
        SortedDictionary<int, LoadItemData> loadDic = null;

        #region 外部调用：初始化加载场景所需Prefabs、加载一个unity新场景
        /// <summary>
        /// 调用完整的加载步骤
        /// </summary>
        public void Init()
        {
            //1.注册事件
            Start();
            //2.设置加载完成事件
            SetOnObjLoaded(InstantiateObj);
            //3.加载场景所需的所有prefab
            LoadAllScenePrefabs();
        }


        /// <summary>
        /// GC并加载下一个新场景(Unity切换方式)
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadNewScene(string sceneName)
        {
            System.GC.Collect();
            SceneManager.LoadScene(sceneName);
        }

        #endregion

        #region 事件注册与处理

        /// <summary>
        /// 事件加载与注册
        /// </summary>
        protected override void Start()
        {
            base.Start();
            AddNewMsgIds((ushort)SceneLoadEvent.LoadScene);
            RegistSelf();
        }

        /// <summary>
        /// 处理加载场景事件
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            base.ProcessEvent(msg);

            if (msg.MsgId == (ushort)SceneLoadEvent.LoadScene)
            {
                MsgString msgString = msg as MsgString;
                LoadNewScene(msgString.Str);
            }
        }
        #endregion

        #region 读取记录，请求加载prefab


        /// <summary>
        /// 读取记录文件，得到场景所需的prefab信息
        /// </summary>
        /// <returns></returns>
        private SortedDictionary<int, LoadItemData> GetScenePrefabNames()
        {
            string filePath = new StringBuilder(PathTool.GetAssetBundlePath()).Append('/').Append(SceneManager.GetActiveScene().name).Append("_loadRecord.xml").ToString();
            SortedDictionary<int, LoadItemData> prefabs = new SortedDictionary<int, LoadItemData>();
            XmlReader reader = XmlReader.Create(filePath);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("LoadItem"))
                {
                    XElement ele = XElement.ReadFrom(reader) as XElement;
                    LoadItemData loadItem = new LoadItemData();
                    loadItem.initialOrder = int.Parse(ele.Attribute("InitialOrder").Value);
                    loadItem.isActive = bool.Parse(ele.Attribute("IsActive").Value);
                    loadItem.prefabABName = ele.Attribute("PrefabABPath").Value;
                    loadItem.organizeDataABName = ele.Attribute("OrganizeDataABPath").Value;
                    prefabs.Add(loadItem.initialOrder, loadItem);
                }
            }
            reader.Close();

            return prefabs;
        }

        /// <summary>
        /// 加载场景初始所需的所有prefab
        /// </summary>
        private void LoadAllScenePrefabs()
        {
            loadDic = GetScenePrefabNames();
            loadedObjRecord.Clear();
            cacheObjs.Clear();

            for (int i = 0; i < loadDic.Count; i++)
            {
                LoadItemData loadItem = loadDic[i];
                //1.记录请求加载的资源
                loadedObjRecord.Add(loadDic[i].prefabABName, false);
                loadedObjRecord.Add(loadDic[i].organizeDataABName, false);
                //2.加载prefab本身
                LoadRequest(loadItem.prefabABName);
                //3.加载prefab对应的organize data(Scriptable Object)
                LoadRequest(loadItem.organizeDataABName);
            }
            cacheObjs.Clear();
        }


        /// <summary>
        /// 请求加载资源
        /// </summary>
        /// <param name="assetName"></param>
        private void LoadRequest(string assetName)
        {
            string[] nameCompose = assetName.Split(' ');
            LoadObjRequest(nameCompose[0], nameCompose[1]);
        }

        #endregion

        #region 实例化资源

        /// <summary>
        /// 实例化资源
        /// </summary>
        /// <param name="abAndAssetName"></param>
        /// <param name="obj"></param>
        private void InstantiateObj(string abAndAssetName, Object obj)
        {
            if (!loadedObjRecord.ContainsKey(abAndAssetName))
                return;

            if (!cacheObjs.ContainsKey(abAndAssetName))
                cacheObjs.Add(abAndAssetName, obj);

            //↓记录已加载的资源
            loadedObjRecord[abAndAssetName] = true;
            //↓如果都已经加载了，就全部实例化
            if (AllLoaded())
            {
                //1.实力化所有GO
                for (int i = 0; i < loadDic.Count; i++)
                {
                    InstantiateSingleObj(loadDic[i]);
                }
                //2.卸载对应的包
                UnloadUnNeededAB();
                //3.清空记录
                cacheObjs.Clear();
                loadDic.Clear();
                loadedObjRecord.Clear();
            }
        }

        /// <summary>
        /// 是否所有所需的资源都已经加载完成了
        /// </summary>
        /// <returns></returns>
        private bool AllLoaded()
        {
            foreach (bool b in loadedObjRecord.Values)
            {
                if (!b)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 实例化单个GO
        /// </summary>
        /// <param name="loadItem"></param>
        private void InstantiateSingleObj(LoadItemData loadItem)
        {
            GameObject go = GameObject.Instantiate(cacheObjs[loadItem.prefabABName]) as GameObject;
            go.name = NameTool.GetRealName(loadItem.prefabABName);
            EntityInfo info = cacheObjs[loadItem.organizeDataABName] as EntityInfo;
            go.GetComponent<EntityConfig>().InitSelf(info);
            RegistGameObject(go);
            if (go.GetComponent<ModuleBase>() != null)
            {
                go.GetComponent<ModuleBase>().Regist();
            }
        }

        /// <summary>
        /// 遍历实例化的GameObject及其层次结构，将他们注册到各自的管理模块
        /// </summary>
        /// <param name="root"></param>
        private void RegistGameObject(GameObject root)
        {
            switch (LayerMask.LayerToName(root.layer))
            {
                case "UI":
                    if (root.GetComponent<UIRegister>() != null)
                        root.GetComponent<UIRegister>().Regist();
                    break;
                case "Building":
                    //TODO
                    break;
            }

            for (int i = 0; i < root.transform.childCount; i++)
            {
                RegistGameObject(root.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 卸载不再需要的AssetBundle
        /// </summary>
        private void UnloadUnNeededAB()
        {
            List<string> unloadABs = new List<string>();
            foreach (LoadItemData loadItem in loadDic.Values)
            {
                string bundleName = NameTool.GetBundleName(loadItem.prefabABName);
                if (!unloadABs.Contains(bundleName))
                    unloadABs.Add(bundleName);
            }

            MgrCenter.Instance.SendMsg(Msgs.GetMsgUnloadAssetBundles((ushort)AssetLoadEvent.UnloadBundles, unloadABs.ToArray(), false));
        }
        #endregion
    }
}
