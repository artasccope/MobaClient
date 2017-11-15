using GameFW.Asset;
using GameFW.Core.Msg;
using GameFW.NetClient;
using Protocol.DTO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFW.UI.Select
{
    /// <summary>
    /// 选择英雄UI模块
    /// </summary>
    public class SelectUI : UIBase
    {

        #region 头像图片资源文件记录
        /// <summary>
        /// 头像图片资源文件记录
        /// </summary>
        private string[] spriteNames = new string[10] {
                "select/textures/head.ld|1_head_biz_green.png",
                "select/textures/head.ld|2_head_boss_blue.png",
                "select/textures/head.ld|3_head_fireFighter_brown.png",
                "select/textures/head.ld|4_head_girl_beach.png",
                "select/textures/head.ld|5_head_hobo_skeleton.png",
                "select/textures/head.ld|6_head_nurse_grey.png",
                "select/textures/head.ld|7_head_police_armor.png",
                "select/textures/head.ld|8_head_police_blueCoat.png",
                "select/textures/head.ld|9_head_punk_brown.png",
                "select/textures/head.ld|10_head_robbor_darkBlue.png",
            };
        /// <summary>
        /// 头像图片包
        /// </summary>
        private string[] spriteBundleNames = new string[1] {
            "select/textures/head.ld",
        };

        #endregion

        #region 应用内消息注册、处理
        private int teamPlayerCount;//队伍玩家数

        private void Start()
        {
            base.Regist();
            ushort[] newMsgIds = new ushort[6] {
                (ushort)NetEventSelect.EnterSres,
                (ushort)AssetLoadEvent.AssetLoaded,
                (ushort)NetEventSelect.SomeOneEntered,
                (ushort)SelectUIEvent.HeroPressed,
                (ushort)NetEventSelect.SomeOneSelected,
                (ushort)NetEventSelect.SomeOneReady,
            };

            AddNewMsgIds(newMsgIds);
            RegistSelf();

            teamPlayerCount = PlayerPrefs.GetInt("TeamPlayerCount");
            Initial();
        }

        private int selectedHeroId = -1;//已选择的英雄Id

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public override void ProcessEvent(MsgBase msg)
        {
            switch (msg.MsgId)
            {
                case (ushort)NetEventSelect.EnterSres://服务器允许进入选择
                    MsgSelectRoom msgSelectRoom = msg as MsgSelectRoom;
                    Debug.LogWarning("enter sres");
                    if (msgSelectRoom.selectRoom != null)
                    {
                        this.selectRoom = msgSelectRoom.selectRoom;
                        SetPlayers(this.selectRoom);
                    }
                    InitialSelectHeroGrids();

                    break;
                case (ushort)AssetLoadEvent.AssetLoaded://图片加载完成
                    MsgAssetLoaded msgAssetLoaded = msg as MsgAssetLoaded;
                    string assetIndex = msgAssetLoaded.assetName.Split('_')[0];
                    int index = int.Parse(assetIndex);
                    Texture2D texture = msgAssetLoaded.asset as Texture2D;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    OnSpriteLoaded(index, sprite);

                    break;
                case (ushort)NetEventSelect.SomeOneEntered://有玩家进入了
                    MsgInt msgInt = msg as MsgInt;
                    UpdatePlayerInfoOnEnter(msgInt.Int, true);

                    break;
                case (ushort)SelectUIEvent.HeroPressed://按下了某个英雄
                    MsgInt msgIndex = msg as MsgInt;
                    selectedHeroId = msgIndex.Int;
                    heroSelectGridBox.UIOnSomeHeroSelected(msgIndex.Int);

                    break;
                case (ushort)NetEventSelect.SomeOneSelected://某人选择了英雄
                    MsgSelectDTO msgSelectDTO = msg as MsgSelectDTO;
                    SelectDTO selectDTO = msgSelectDTO.SelectData;
                    if (selectDTO != null)
                    {
                        UpdatePlayerHero(selectDTO.userId, selectDTO.heroId);
                    }
                    if (TeamId(selectDTO.userId) == this.teamId)
                    {
                        UpdateHeroCouldSelect(selectDTO);
                    }

                    break;
                case (ushort)NetEventSelect.SomeOneReady://有人准备了
                    MsgSelectDTO msgSelect = msg as MsgSelectDTO;
                    SelectDTO select = msgSelect.SelectData;
                    UpdatePlayerReady(select.userId, select.isReady);

                    break;
            }
        }

        #endregion

        #region 获得组件、注册事件、初始化列表

        /// <summary>
        /// 初始化
        /// </summary>
        protected void Initial()
        {
            GetHeroHeadSprites();
            ensureSelectButton = GetUIWidgetByWidgetName(UIWidgetNames.SelectEnsureButtonName);
            readyButton = GetUIWidgetByWidgetName(UIWidgetNames.ReadyButtonName);
            ensureSelectButton.GetComponent<UIRegister>().AddButtonListener(OnEnsureSelectPressed);
            readyButton.GetComponent<UIRegister>().AddButtonListener(OnReadyPressed);

            leftList = GetUIWidgetByWidgetName(UIWidgetNames.PlayerLeftListName).transform;
            rightList = GetUIWidgetByWidgetName(UIWidgetNames.PlayerRightListName).transform;
            InitialPlayerList(leftList, this.teamPlayerCount);
            InitialPlayerList(rightList, this.teamPlayerCount);

        }

        #endregion

        #region 英雄头像相关
        private Sprite[] headSprites;//英雄头像对应的Sprite

        /// <summary>
        /// 请求获取英雄头像Sprites
        /// </summary>
        private void GetHeroHeadSprites()
        {
            headSprites = new Sprite[spriteNames.Length];
            for (int i = 0; i < spriteNames.Length; i++)
            {
                string[] nameCompose = spriteNames[i].Split('|');
                SendMsg(Msgs.GetMsgAssetLoadRequest((ushort)AssetLoadEvent.LoadRequest, nameCompose[0], nameCompose[1]));
            }
        }

        /// <summary>
        /// 当Sprite load完成时调用
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sprite"></param>
        private void OnSpriteLoaded(int index, Sprite sprite)
        {
            if (sprite != null && (index >= 1 && index <= headSprites.Length))
            {
                headSprites[index - 1] = sprite;

                if (IfAllHeadSpritesLoaded())
                {
                    SendMsg(Msgs.GetMsgUnloadAssetBundles((ushort)AssetLoadEvent.UnloadBundles, spriteBundleNames, false));
                    //到这里有了所有需要的资源了,再发送进入的请求,这样服务器再返回的消息需要的资源已经有了，就可以马上进行设置
                    SendMsg(Msgs.GetMsgBase((ushort)NetEventSelect.EnterRequest));
                }
            }
        }

        /// <summary>
        /// 是否全部load完成了
        /// </summary>
        /// <returns></returns>
        private bool IfAllHeadSpritesLoaded()
        {
            for (int i = 0; i < headSprites.Length; i++)
            {
                if (headSprites[i] == null)
                    return false;
            }

            return true;
        }

        #endregion

        #region 准备、确认选择按钮
        private GameObject ensureSelectButton;//确认选择按钮
        private GameObject readyButton;//准备按钮

        /// <summary>
        /// 确认按钮
        /// </summary>
        private void OnEnsureSelectPressed()
        {
            //TODO 取消选择

            SendMsg(Msgs.GetMsgInt((ushort)NetEventSelect.SelectRequest, selectedHeroId));
        }

        /// <summary>
        /// 准备按钮
        /// </summary>
        private void OnReadyPressed()
        {
            if (selectedHeroId != -1)
            {
                readyButton.GetComponent<Button>().enabled = false;
                SendMsg(Msgs.GetMsgBase((ushort)NetEventSelect.ReadyRequest));
            }
        }

        #endregion

        #region 玩家列表相关
        private Transform leftList;//左边的玩家列表
        private Transform rightList;//右边的玩家列表
        /// <summary>
        /// 初始化玩家列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="teamPlayerCount"></param>
        private void InitialPlayerList(Transform list, int teamPlayerCount)
        {
            if (teamPlayerCount < 1 || teamPlayerCount > 5)
            {
                return;
            }

            for (int i = teamPlayerCount; i < list.childCount; i++)
            {
                list.GetChild(i).gameObject.SetActive(false);
            }
        }

        private SelectRoomDTO selectRoom;//选择房间信息

        private int selfUserId;//自己的User id
        private int teamId;//自己的队伍Id
        /// <summary>
        /// 设置两队所有玩家的UI
        /// </summary>
        /// <param name="room"></param>
        private void SetPlayers(SelectRoomDTO room)
        {
            selfUserId = room.SelfUserId;
            teamId = room.GetTeam(room.SelfUserId);
            if (teamId == 1)
            {
                SetTeam(leftList, room.teamOne);
                SetTeam(rightList, room.teamTwo);
            }
            else if (teamId == 2)
            {
                SetTeam(leftList, room.teamTwo);
                SetTeam(rightList, room.teamOne);
            }
        }

        /// <summary>
        /// 保存了所有玩家id和信息格的对应
        /// </summary>
        private Dictionary<int, SelectPlayerInfoGrid> selectInfoLists = new Dictionary<int, SelectPlayerInfoGrid>();

        /// <summary>
        /// 设置一个队伍
        /// </summary>
        /// <param name="playerList"></param>
        /// <param name="team"></param>
        private void SetTeam(Transform playerList, SelectDTO[] team)
        {
            SelectPlayerInfoList list = playerList.GetComponent<SelectPlayerInfoList>();
            if (list != null)
            {
                Dictionary<int, SelectPlayerInfoGrid> grids = list.InitialPlayerInfo(team, this.headSprites);
                foreach (KeyValuePair<int, SelectPlayerInfoGrid> p in grids)
                {
                    if (!selectInfoLists.ContainsKey(p.Key))
                        selectInfoLists.Add(p.Key, p.Value);
                }
            }
        }

        #endregion

        #region 更新玩家信息格

        /// <summary>
        /// 玩家进入时更新
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isEntered"></param>
        private void UpdatePlayerInfoOnEnter(int userId, bool isEntered)
        {
            if (selectInfoLists.ContainsKey(userId))
                selectInfoLists[userId].UpdateInfo(isEntered);
        }

        /// <summary>
        /// 更新玩家的英雄信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="heroId"></param>
        private void UpdatePlayerHero(int userId, int heroId)
        {
            if (selectInfoLists.ContainsKey(userId))
                selectInfoLists[userId].UpdateSprite(heroId);
        }

        /// <summary>
        /// 更新玩家的准备信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isReady"></param>
        private void UpdatePlayerReady(int userId, bool isReady)
        {
            if (selectInfoLists.ContainsKey(userId))
                selectInfoLists[userId].UpdateStatusInfo(isReady);
        }
        #endregion

        #region 英雄选择列表相关

        /// <summary>
        /// 更新可选择的英雄列表
        /// </summary>
        /// <param name="selectDTO"></param>
        private void UpdateHeroCouldSelect(SelectDTO selectDTO)
        {
            SelectDTO[] team = GetTeam(this.teamId);
            for (int i = 0; i < team.Length; i++)
            {
                if (team[i].userId == selectDTO.userId)
                    team[i] = selectDTO;
            }

            heroCouldSelected.Clear();
            for (int i = 0; i < heroList.Length; i++)
            {
                heroCouldSelected.Add(heroList[i]);
            }
            foreach (SelectDTO item in team)
            {
                if (heroCouldSelected.Contains(item.heroId))
                    heroCouldSelected.Remove(item.heroId);
            }
            heroSelectGridBox.UpdateSelectHeroGrids(heroCouldSelected);
        }

        /// <summary>
        /// 得到team id对应的队伍全部信息
        /// </summary>
        /// <param name="tId"></param>
        /// <returns></returns>
        private SelectDTO[] GetTeam(int tId)
        {
            if (tId == 1)
                return selectRoom.teamOne;
            else
                return selectRoom.teamTwo;
        }

        /// <summary>
        /// 目前玩家可选择的英雄集合
        /// </summary>
        private HashSet<int> heroCouldSelected = new HashSet<int>();

        private int TeamId(int userId)
        {
            return selectRoom.GetTeam(userId);
        }

        /// <summary>
        /// 得到玩家目前有的英雄列表
        /// </summary>
        /// <returns></returns>
        private int[] GetHeroList()
        {
            if (teamId == 1)
            {
                foreach (SelectDTO dto in selectRoom.teamOne)
                {
                    if (dto.userId == selfUserId)
                        return dto.heroList;
                }
            }
            else if (teamId == 2)
            {
                foreach (SelectDTO dto in selectRoom.teamTwo)
                {
                    if (dto.userId == selfUserId)
                        return dto.heroList;
                }
            }

            return null;
        }

        private int[] heroList;//英雄列表缓存
        private HeroSelectGridBox heroSelectGridBox;//英雄选择GridBox

        /// <summary>
        /// 初始化英雄选择Grid Box
        /// </summary>
        private void InitialSelectHeroGrids()
        {
            heroList = GetHeroList();
            for (int i = 0; i < heroList.Length; i++)
            {
                heroCouldSelected.Add(heroList[i]);
            };
            heroSelectGridBox = GetUIWidgetByWidgetName(UIWidgetNames.HeroGridBoxName).GetComponent<HeroSelectGridBox>();
            heroSelectGridBox.InstantiateGrids(heroCouldSelected, headSprites);
        }

        #endregion
    }
}
