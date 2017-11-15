namespace GameFW.Entity.Skill
{
    /// <summary>
    /// 每个角色有一组包括攻击、回血等等主动释放的技能
    /// 每个技能都有通用的模型
    /// 1.技能前置条件检查 client触发、server验证 (公共CD?)
    /// 2.技能引导(可打断?) client运行，运行完成发server结果 (攻击挥刀相当于技能引导)
    /// 3.server执行技能消耗扣除,发技能成功给client
    /// 4.client收到后、技能播放
    /// 5.同时、技能效果结算并且施加  
    /// 6.记录这组技能及技能效果以回滚
    /// </summary>
    public interface ISkill
    {
        bool CheckCondition();
        bool CheckCost();
        void ExecuteCost();
        void Boot();
        void Execute();
    }
}
