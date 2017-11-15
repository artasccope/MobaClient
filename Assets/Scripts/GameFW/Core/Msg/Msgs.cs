using Protocol.DTO;
using Protocol.DTO.Fight;
using Protocol.DTO.Fight.Instance;
using UnityEngine;

namespace GameFW.Core.Msg
{
    public class Msgs
    {
        private static MsgBase msgBase = new MsgBase();
        public static MsgBase GetMsgBase(ushort msgId) {
            msgBase.MsgId = msgId;
            return msgBase;
        }

        private static MsgUnloadAssetBundles msgUnloadAssetBundles = new MsgUnloadAssetBundles();
        public static MsgUnloadAssetBundles GetMsgUnloadAssetBundles(ushort msgId, string[] bundleNames, bool ifUnloadObjs) {
            msgUnloadAssetBundles.SetMsgUnloadAssetBundles(msgId, bundleNames, ifUnloadObjs);
            return msgUnloadAssetBundles;
        }

        private static MsgAssetLoaded msgAssetLoaded = new MsgAssetLoaded();



        public static MsgAssetLoaded GetMsgAssetLoaded(ushort msgId, string bundleName, string assetName, UnityEngine.Object asset) {
            msgAssetLoaded.SetMsgAssetLoaded(msgId, bundleName, assetName, asset);
            return msgAssetLoaded;
        }

        private static MsgAssetLoadRequest msgAssetLoadRequest = new MsgAssetLoadRequest();
        public static MsgAssetLoadRequest GetMsgAssetLoadRequest(ushort msgId, string bundleName, string assetName) {
            msgAssetLoadRequest.SetMsgAssetLoadRequest(msgId, bundleName, assetName);
            return msgAssetLoadRequest;
        }

        private static MsgTransform msgTransform = new MsgTransform();
        public static MsgTransform GetMsgTransform(ushort msgId, Transform transform) {
            msgTransform.SetMsgTransform(msgId, transform);
            return msgTransform;
        }

        private static MsgBundleLoadProgress msgBundleLoadProgress = new MsgBundleLoadProgress();
        public static MsgBundleLoadProgress GetMsgBundleLoadProgress(ushort msgId, string bundleName, float progress) {
            msgBundleLoadProgress.SetBundleLoadProgress(msgId, bundleName, progress);
            return msgBundleLoadProgress;
        }

        private static MsgString msgString = new MsgString();
        public static MsgString GetMsgString(ushort msgId, string str) {
            msgString.SetMsgString(msgId, str);
            return msgString;
        }

        private static MsgUserDTO msgUserDTO = new MsgUserDTO();
        public static MsgUserDTO GetMsgUserDTO(ushort msgId, UserDTO userDTO) {
            msgUserDTO.SetMsgUserDTO(msgId, userDTO);
            return msgUserDTO;
        }

        private static MsgSelectRoom msgSelectRoom = new MsgSelectRoom();

        private static MsgMoveRequest msgMoveRequest = new MsgMoveRequest();
        internal static MsgBase GetMsgMoveRequest(ushort msgId, int instanceId, Vector3 sourcePos, Vector3 targetPos)
        {
            msgMoveRequest.SetMsgMoveRequest(msgId, instanceId, sourcePos, targetPos);
            return msgMoveRequest;
        }

        public static MsgSelectRoom GetMsgSelectRoom(ushort msgId, SelectRoomDTO selectRoomDTO)
        {
            msgSelectRoom.SetMsgSelectRoom(msgId, selectRoomDTO);
            return msgSelectRoom;
        }

        private static MsgAccount msgAccount = new MsgAccount();
        public static MsgAccount GetMsgAccount(ushort msgId, string account, string password) {
            msgAccount.SetMsgAccount(msgId, account, password);
            return msgAccount;
        }

        private static MsgInt msgInt = new MsgInt();
        public static MsgInt GetMsgInt(ushort msgId, int i) {
            msgInt.SetMsgInt(msgId, i);
            return msgInt;
        }

        private static MsgSelectDTO msgSelectDTO = new MsgSelectDTO();
        public static MsgSelectDTO GetMsgSelectDTO(ushort msgId, SelectDTO selectDTO) {
            msgSelectDTO.SetMsgSelectDTO(msgId, selectDTO);
            return msgSelectDTO;
        }

        private static MsgDelayAndFloating msgDelayAndFloating = new MsgDelayAndFloating();
        public static MsgDelayAndFloating GetMsgDelayAndFloating(ushort msgId, DelayAndFloating df)
        {
            msgDelayAndFloating.SetMsgDelayAndFloating(msgId, df);
            return msgDelayAndFloating;
        }

        private static MsgBuildingCreate msgBuildingCreate = new MsgBuildingCreate();
        public static MsgBuildingCreate GetMsgBuildingCreate(ushort msgId, BuildingInstance dto, bool isHost)
        {
            msgBuildingCreate.SetBuildingCreate(msgId, dto, isHost);
            return msgBuildingCreate;
        }

        private static MsgNPCCreate msgNPCCreate = new MsgNPCCreate();
        public static MsgNPCCreate GetMsgNPCCreate(ushort msgId, AbsFightInstance instance, bool isHost)
        {
            msgNPCCreate.SetNPCCreate(msgId, instance, isHost);
            return msgNPCCreate;
        }

        private static MsgIntArr msgIntArr = new MsgIntArr();



        public static MsgBase GetMsgIntArr(ushort msgId, int[] arr)
        {
            msgIntArr.SetMsgIntArr(msgId, arr);
            return msgIntArr;
        }

        private static MsgPosSync msgPosSync = new MsgPosSync();
        public static MsgPosSync GetMsgPosSync(ushort msgId, int instanceId, Vector3 pos, Vector3 dir, long timeStamp) {
            msgPosSync.SetMsgPosSync(msgId, instanceId, pos, dir, timeStamp);
            return msgPosSync;
        }

        private static MsgAtkRequest msgAtkRequest = new MsgAtkRequest();
        public static MsgAtkRequest GetMsgAtkRequest(ushort msgId, int attackerId, int targetId)
        {
            msgAtkRequest.SetMsgAtkRequest(msgId, attackerId, targetId);
            return msgAtkRequest;
        }

        public static MsgT<PosDTO> msgPos = new MsgT<PosDTO>();
        public static MsgT<PosDTO> GetMsgPos(ushort msgId, PosDTO posDTO) {
            msgPos.SetMsgT(msgId, posDTO);
            return msgPos;
        }

        public static MsgKV<int, Vector3> msgEntityPos = new MsgKV<int, Vector3>();
        public static MsgKV<int, Vector3> GetMsgEntityPos(ushort msgId, int entityId, Vector3 pos)
        {
            msgEntityPos.SetMsgKV(msgId, entityId, pos);
            return msgEntityPos;
        }

        public static MsgHUD msgHUD = new MsgHUD();
        public static MsgHUD GetMsgHUD(ushort msgId, int entityId, Vector3 pos, string name, float percent)
        {
            msgHUD.SetMsgHUD(msgId, entityId, pos, name, percent);
            return msgHUD;
        }

        public static MsgKV<int, float> msgIntFloat = new MsgKV<int, float>();
        public static MsgKV<int, float> GetMsgIntFloat(ushort msgId, int i, float f) {
            msgIntFloat.SetMsgKV(msgId, i, f);
            return msgIntFloat;
        }
    }
}
