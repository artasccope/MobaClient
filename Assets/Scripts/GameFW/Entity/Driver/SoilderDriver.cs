using Protocol.DTO.Fight.Instance;
using UnityEngine;

namespace GameFW.Entity.Driver
{
    /// <summary>
    /// 士兵驱动类
    /// </summary>
    public class SoilderDriver : FightDriver
    {
        /// <summary>
        /// 攻击范围
        /// </summary>
        public float AtkRange { get { return this.fightInstance.atkRange; } }
        /// <summary>
        /// 视野范围
        /// </summary>
        public float EyeRange { get { return this.fightInstance.eyeRange; } }
        /// <summary>
        /// 唯一Id
        /// </summary>
        public int Id { get { return this.fightInstance.instanceId; } }
        /// <summary>
        /// 移动速度
        /// </summary>
        public float Speed { get { return this.fightInstance.speed; } }
        private Animator animator;

        #region 初始化
        public override void Initial(AbsFightInstance fightInstance)
        {
            base.Initial(fightInstance);
            animator = gameObject.GetComponent<Animator>();
        }
        #endregion

        #region 动作相关

        /// <summary>
        /// 播放发呆动画
        /// </summary>
        public void Idle()
        {
            animator.SetFloat("Speed_f", 0f);
        }

        /// <summary>
        /// 只播放移动动画
        /// </summary>
        public void Move()
        {
            animator.SetFloat("Speed_f", fightInstance.speed);
        }

        /// <summary>
        /// 播放攻击动画
        /// </summary>
        public void PlayAttack() {

        }

        #endregion
    }
}
