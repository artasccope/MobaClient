using GameFW.Core.Msg;
using Protocol.DTO.Fight.Instance;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;
using System.Xml.Linq;
using Protocol.DTO.Fight;
using GameFW.GameMgr;
using GameFW.Entity.Driver;
using UnityEngine.AI;
using GameFW.Asset;
using GameFW.Core;
using GameFW.Persistence;
using GameFW.Core.Base;

namespace GameFW.Entity
{
    /// <summary>
    /// 游戏Entity加载器
    /// </summary>
    public class EntityLoader : ModuleBase
    {
        #region 单例
        private static EntityLoader instance;
        private static System.Object obj = new System.Object();
        public static EntityLoader Instance
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
                            instance = o.AddComponent<EntityLoader>();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        #region 注册、处理消息
        public override void Regist()
        {
            base.Regist();
            ushort[] newMsgs = new ushort[3] {
                (ushort)EntityEnum.CreateEntity,
                (ushort)EntityEnum.CreateBuilding,
                (ushort)AssetLoadEvent.AssetLoaded,
            };
            AddNewMsgIds(newMsgs);
            RegistSelf();
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)EntityEnum.CreateEntity://创建entity
                    MsgNPCCreate msgNPCCreate = msg as MsgNPCCreate;
                    LoadEntity(msgNPCCreate.npcInstance, msgNPCCreate.isHost);

                    break;
                case (ushort)EntityEnum.CreateBuilding://创建建筑
                    MsgBuildingCreate msgBuildingCreate = msg as MsgBuildingCreate;
                    LoadBuilding(msgBuildingCreate.buildingCreateDTO, msgBuildingCreate.isHost);

                    break;
                case (ushort)AssetLoadEvent.AssetLoaded://资源加载完成，实例化entity
                    MsgAssetLoaded msgAssetLoaded = msg as MsgAssetLoaded;
                    string name = new StringBuilder(msgAssetLoaded.bundleName).Append(" ").Append(msgAssetLoaded.assetName).ToString();
                    InstantiateEntity(name, msgAssetLoaded.asset);

                    break;
            }
        }
        #endregion

        #region 初始化、读取预制信息
        public void Init()
        {
            ReadNPCAssetXML();
            Regist();
        }

        private Dictionary<int, string> prefabNames = new Dictionary<int, string>();
        private void ReadNPCAssetXML()
        {
            prefabNames.Clear();
            string filePath = new StringBuilder(PathTool.GetAssetBundlePath()).Append('/').Append(SceneManager.GetActiveScene().name).Append("_runtimePrefabRecord.xml").ToString();
            XmlReader reader = XmlReader.Create(filePath);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("RuntimeLoadItem"))
                {
                    XElement ele = XElement.ReadFrom(reader) as XElement;
                    int categoryId = int.Parse(ele.Attribute("CategoryId").Value);
                    int specieId = int.Parse(ele.Attribute("SpecieId").Value);
                    int key = categoryId.GetHashCode() + specieId.GetHashCode();
                    if (!prefabNames.ContainsKey(key))
                    {
                        prefabNames.Add(key, ele.Attribute("PrefabABPath").Value);
                    }
                }
            }
            reader.Close();
        }
        #endregion

        #region 加载entity、建筑
        /// <summary>
        /// 当实例所需的资源被加载完成时,使用这个字典来创建对应实例
        /// </summary>
        private Dictionary<string, HashSet<AbsFightInstance>> instanceDic = new Dictionary<string, HashSet<AbsFightInstance>>();
        /// <summary>
        /// 记载一个id对应的实例是否运行AI
        /// </summary>
        private HashSet<int> hasAIDic = new HashSet<int>();

        /// <summary>
        /// 加载一个entity
        /// </summary>
        /// <param name="fightInstance"></param>
        /// <param name="isHost"></param>
        private void LoadEntity(AbsFightInstance fightInstance, bool isHost)
        {
            int key = fightInstance.fightModel.category.GetHashCode() + fightInstance.fightModel.specieId.GetHashCode();
            if (prefabNames.ContainsKey(key))
            {
                string abAssetName = prefabNames[key];
                string[] nameCompose = abAssetName.Split(' ');
                if (!instanceDic.ContainsKey(abAssetName))
                    instanceDic.Add(abAssetName, new HashSet<AbsFightInstance>());
                if (!instanceDic[abAssetName].Contains(fightInstance))
                    instanceDic[abAssetName].Add(fightInstance);

                if (isHost && fightInstance.teamId == GameRuntimeData.teamId)
                {//当本客户端是主机且这个实例是自己队伍的，那么这个实例运行AI
                    hasAIDic.Add(fightInstance.instanceId);
                }

                SendMsg(Msgs.GetMsgAssetLoadRequest((ushort)AssetLoadEvent.LoadRequest, nameCompose[0], nameCompose[1]));
            }
        }

        /// <summary>
        /// 加载一个建筑
        /// </summary>
        /// <param name="buildingInstance"></param>
        /// <param name="isHost"></param>
        private void LoadBuilding(BuildingInstance buildingInstance, bool isHost)
        {
            int bKey = buildingInstance.fightModel.category.GetHashCode() + buildingInstance.fightModel.specieId.GetHashCode();
            if (prefabNames.ContainsKey(bKey))
            {
                string abAssetName = prefabNames[bKey];
                string[] nameCompose = abAssetName.Split(' ');
                if (!instanceDic.ContainsKey(abAssetName))
                    instanceDic.Add(abAssetName, new HashSet<AbsFightInstance>());
                if (!instanceDic[abAssetName].Contains(buildingInstance))
                    instanceDic[abAssetName].Add(buildingInstance);

                if (isHost && buildingInstance.teamId == GameRuntimeData.teamId)
                {
                    hasAIDic.Add(buildingInstance.instanceId);
                }

                SendMsg(Msgs.GetMsgAssetLoadRequest((ushort)AssetLoadEvent.LoadRequest, nameCompose[0], nameCompose[1]));
            }
        }

        #endregion

        #region 创建实例

        /// <summary>
        /// 当资源加载完成时创建实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        private void InstantiateEntity(string name, Object asset)
        {
            if (instanceDic.ContainsKey(name))
            {
                foreach (AbsFightInstance fightInstance in instanceDic[name])
                {
                    if (hasAIDic.Contains(fightInstance.instanceId))
                    {
                        CreateEntityInstance(fightInstance, asset, true);
                    }
                    else
                    {
                        CreateEntityInstance(fightInstance, asset, false);
                    }
                }
                instanceDic[name].Clear();
            }
        }

        /// <summary>
        /// 创建实体实例
        /// </summary>
        /// <param name="fightInstance"></param>
        /// <param name="obj"></param>
        /// <param name="hasAI"></param>
        private void CreateEntityInstance(AbsFightInstance fightInstance, UnityEngine.Object obj, bool hasAI)
        {
            GameObject entity = null;
            switch (fightInstance.fightModel.category)
            {
                case (byte)ModelType.Building:
                    BuildingInstance buildingInstance = fightInstance as BuildingInstance;
                    entity = GameObject.Instantiate(obj, new Vector3(buildingInstance.posX, buildingInstance.posY, buildingInstance.posZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                    entity.transform.eulerAngles = new Vector3(buildingInstance.eAngleX, buildingInstance.eAngleY, buildingInstance.eAngleZ);
                    entity.transform.localScale = new Vector3(buildingInstance.scaleX, buildingInstance.scaleY, buildingInstance.scaleZ);
                    break;
                case (byte)ModelType.Hero:
                    entity = GameObject.Instantiate(obj, new Vector3(fightInstance.posX, fightInstance.posY, fightInstance.posZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                    break;
                case (byte)ModelType.Creature:
                    entity = GameObject.Instantiate(obj, new Vector3(fightInstance.posX, fightInstance.posY, fightInstance.posZ), Quaternion.Euler(0, 0, 0)) as GameObject;
                    break;
            }

            if (entity != null)
            {
                if (fightInstance.teamId == 1)
                    entity.layer = LayerIds.TeamOneLayer;
                else
                    entity.layer = LayerIds.TeamTwoLayer;
            }

            InitialScripts(entity, fightInstance);
            InitialAI(hasAI, entity, fightInstance);
        }

        #endregion

        #region 给加载完成的实例添加脚本

        /// <summary>
        /// 初始化Register和Driver脚本
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fightInstance"></param>
        private void InitialScripts(GameObject entity, AbsFightInstance fightInstance)
        {
            //Data Model setting
            EntityRegister register = entity.GetComponent<EntityRegister>();
            if (register == null)
                register = entity.AddComponent<EntityRegister>();
            register.Regist(fightInstance.instanceId);
            //register to aoi
            register.RegistAOI(fightInstance.instanceId, entity);

            FightDriver fightDriver = entity.GetComponent<FightDriver>();
            if (fightDriver == null)
                fightDriver = entity.AddComponent<FightDriver>();
            fightDriver.Initial(fightInstance);
        }

        /// <summary>
        /// 初始化AI脚本
        /// </summary>
        /// <param name="hasAI"></param>
        /// <param name="entity"></param>
        /// <param name="fightInstance"></param>
        private void InitialAI(bool hasAI, GameObject entity, AbsFightInstance fightInstance)
        {
            if (hasAI)
            {
                FightAgent fightAgent = entity.GetComponent<FightAgent>();
                switch (fightInstance.fightModel.category)
                {
                    case (byte)ModelType.Building:
                        if (entity.GetComponent<NavMeshAgent>() != null)
                            Destroy(entity.GetComponent<NavMeshAgent>());

                        fightAgent = AddBuildingAgentScript(entity, fightInstance.fightModel.specieId);
                        break;
                    case (byte)ModelType.Creature:
                        if (entity.GetComponent<NavMeshAgent>() == null)
                            entity.AddComponent<NavMeshAgent>();
                        break;
                    case (byte)ModelType.Hero:
                        break;
                }
                if (entity.GetComponent<PosSyncDriver>() != null)
                    Destroy(entity.GetComponent<PosSyncDriver>());

                if (fightAgent != null)
                {
                    //Data Model setting
                    fightAgent.Id = fightInstance.instanceId;
                    //start ai
                    fightAgent.Initial();
                    fightAgent.StartAI();
                }
            }
            else
            {
                if (entity.GetComponent<FightAgent>() != null)
                {
                    Destroy(entity.GetComponent<FightAgent>());
                }
                if (entity.GetComponent<NavMeshAgent>() != null)
                {
                    Destroy(entity.GetComponent<NavMeshAgent>());
                }
                switch (fightInstance.fightModel.category)
                {
                    case (byte)ModelType.Building:
                        if (entity.GetComponent<PosSyncDriver>() != null)
                            Destroy(entity.GetComponent<PosSyncDriver>());
                        break;
                    case (byte)ModelType.Creature:
                    case (byte)ModelType.Hero:
                        if (entity.GetComponent<PosSyncDriver>() == null)
                            entity.AddComponent<PosSyncDriver>();
                        break;
                }
            }
        }

        /// <summary>
        /// 给建筑添加Agent脚本
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="specieId"></param>
        /// <returns></returns>
        private FightAgent AddBuildingAgentScript(GameObject entity, int specieId)
        {
            FightAgent fightAgent = null;
            switch (specieId)
            {
                case (int)BuildingType.defenseTower:
                case (int)BuildingType.advancedDFTower:
                    fightAgent = entity.AddComponent<DefenseTowerAgent>();
                    break;

                case (int)BuildingType.headQuarter:
                case (int)BuildingType.barracks:
                    fightAgent = entity.AddComponent<BuildingAgent>();
                    break;
            }

            return fightAgent;
        }

        #endregion

        #region 消息类型

        protected override void SetMsgType()
        {
            this.msgType = MsgType.Entity;
        }
        #endregion
    }
}
