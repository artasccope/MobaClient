
namespace GameFW.GameMgr
{
    public interface IGameMgr
    {
        void Start();
        void End();
        void Pause();
        void Continue();
        //有的游戏可能没有胜利、失败、或平局，那么可以将它放在子类中
    }
}
