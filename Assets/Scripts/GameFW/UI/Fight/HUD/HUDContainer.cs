using CommonTools;
using GameFW.Asset;
using GameFW.Core.Msg;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFW.UI.Fight.HUD
{
    /// <summary>
    /// HUD容器
    /// </summary>
    public class HUDContainer : UIBase
    {
        /// <summary>
        /// HUD回收缓存
        /// </summary>
        private ObjectPool<EntityHUD> hudCache = new ObjectPool<EntityHUD>();

        private string hudAssetABName = "fight/ui/instantiate.ld";
        private string hudAssetName = "HUD.prefab";
        private Object hudAsset;

        #region 应用内消息注册、处理
        private void Start()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[6] {
                (ushort)AssetLoadEvent.AssetLoaded,
                (ushort)HUDEvent.CreateHUD,
                (ushort)HUDEvent.HideHUD,
                (ushort)HUDEvent.DestroyHUD,
                (ushort)HUDEvent.UpdateHUDPos,
                (ushort)HUDEvent.UpdateHUDHp,
            };

            AddNewMsgIds(newMsgIds);
            RegistSelf();
            Initial();
        }

        /// <summary>
        /// entity Id对应的HUD
        /// </summary>
        private Dictionary<int, EntityHUD> hudDic = new Dictionary<int, EntityHUD>();
        /// <summary>
        /// 加载HUD的记录(在asset加载之前累积的)
        /// </summary>
        private Dictionary<int, HUDInfo> loadHUDDic = new Dictionary<int, HUDInfo>();
        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)AssetLoadEvent.AssetLoaded://HUD prefab加载完成后，创建HUD
                    MsgAssetLoaded msgAssetLoaded = msg as MsgAssetLoaded;
                    if (msgAssetLoaded.bundleName == hudAssetABName && msgAssetLoaded.assetName == hudAssetName)
                    {
                        this.hudAsset = msgAssetLoaded.asset;
                        foreach (HUDInfo hudInfo in loadHUDDic.Values)
                        {
                            CreateHUD(hudAsset, hudInfo.id, hudInfo.name, hudInfo.percent, hudInfo.pos);
                        }
                        loadHUDDic.Clear();
                    }
                    break;
                case (ushort)HUDEvent.CreateHUD:
                    MsgHUD msgHUD = msg as MsgHUD;
                    if (hudAsset == null)
                    {
                        if (loadHUDDic.Count == 0)
                            SendMsg(Msgs.GetMsgAssetLoadRequest((ushort)AssetLoadEvent.LoadRequest, hudAssetABName, hudAssetName));

                        if (!loadHUDDic.ContainsKey(msgHUD.Id))
                            loadHUDDic.Add(msgHUD.Id, new HUDInfo(msgHUD.Name, msgHUD.Id, msgHUD.Percent, msgHUD.Pos));
                    }
                    else
                        CreateHUD(hudAsset, msgHUD.Id, msgHUD.Name, msgHUD.Percent, msgHUD.Pos);

                    break;
                case (ushort)HUDEvent.DestroyHUD://销毁HUD
                    MsgInt msgInt = msg as MsgInt;
                    UnRegisterHUD(msgInt.Int);

                    break;
                case (ushort)HUDEvent.HideHUD:
                    MsgInt msgI = msg as MsgInt;
                    HideHUD(msgI.Int);

                    break;
                case (ushort)HUDEvent.UpdateHUDPos://更新HUD的位置
                    MsgKV<int, Vector3> msgPos = msg as MsgKV<int, Vector3>;
                    if (hudDic.ContainsKey(msgPos.msgKey))
                        UpdateHUDPos(msgPos.msgValue, hudDic[msgPos.msgKey]);

                    break;
                case (ushort)HUDEvent.UpdateHUDHp://更新HUD的hp信息
                    MsgKV<int, float> msgHp = msg as MsgKV<int, float>;
                    if (hudDic.ContainsKey(msgHp.msgKey))
                        hudDic[msgHp.msgKey].UpdateHpBar(msgHp.msgValue);

                    break;
            }
        }


        #endregion

        private void Initial()
        {
        }

        #region 创建、注册、注销、缓存HUD

        /// <summary>
        /// 创建一个HUD
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="percent"></param>
        /// <param name="pos"></param>
        private void CreateHUD(Object asset, int id, string name, float percent, Vector3 pos)
        {
            EntityHUD entityHUD = GetEntityHUD(asset);
            RegisterHUD(id, entityHUD);
            entityHUD.Initial(name, percent);

            UpdateHUDPos(pos, entityHUD);
        }

        /// <summary>
        /// 获得实体HUD
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private EntityHUD GetEntityHUD(Object obj)
        {
            if (hudCache.Count > 0)
            {
                return hudCache.GetObj();
            }
            else
            {
                GameObject go = (GameObject)Instantiate(obj);
                EntityHUD entityHUD = go.GetComponent<EntityHUD>();
                if (entityHUD == null)
                    entityHUD = go.AddComponent<EntityHUD>();

                return entityHUD;
            }
        }

        /// <summary>
        /// 注册一个HUD
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entityHUD"></param>
        private void RegisterHUD(int id, EntityHUD entityHUD)
        {
            if (!hudDic.ContainsKey(id))
            {
                hudDic.Add(id, entityHUD);
            }
            else
            {
                hudDic[id] = entityHUD;
            }
        }

        /// <summary>
        /// 隐藏HUD
        /// </summary>
        /// <param name="entityId"></param>
        private void HideHUD(int entityId)
        {
            if (hudDic.ContainsKey(entityId))
            {
                hudDic[entityId].Hide();
            }
        }

        /// <summary>
        /// 注销一个HUD
        /// </summary>
        /// <param name="id"></param>
        private void UnRegisterHUD(int id)
        {
            if (hudDic.ContainsKey(id))
            {
                EntityHUD entityHUD = hudDic[id];
                ReuseHUD(entityHUD);
                hudDic.Remove(id);
            }
        }

        /// <summary>
        /// 缓存HUD
        /// </summary>
        /// <param name="entityHUD"></param>
        private void ReuseHUD(EntityHUD entityHUD)
        {
            entityHUD.Clear();
            hudCache.PutObj(entityHUD);
        }

        #endregion

        #region 更新HUD位置信息
        /// <summary>
        /// 计算位置,如果在屏幕范围内则显示并更新位置，否则不显示
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="entityHUD"></param>
        private void UpdateHUDPos(Vector3 pos, EntityHUD entityHUD)
        {
            Vector3 posScreen = Camera.main.WorldToScreenPoint(pos);
            if (IsInScreen(posScreen))
            {
                entityHUD.Show();
                entityHUD.UpdatePos(posScreen);
            }
            else
            {
                entityHUD.Hide();
            }
        }

        /// <summary>
        /// 测试hud位置是否在屏幕内
        /// </summary>
        /// <param name="pos2d"></param>
        /// <returns></returns>
        private bool IsInScreen(Vector3 pos)
        {
            return pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height;
        }
        #endregion
    }
}
