
namespace GameFW.GameMgr
{
    public interface IIncidentMgr
    {
        void Victory();
        void Failure();
        void OnBuildingDestroyed(int buildingId);
        void OnHeroKilled(int killderId, int preyId, int totalCount);
        void LaunchTrian();
    }
}
