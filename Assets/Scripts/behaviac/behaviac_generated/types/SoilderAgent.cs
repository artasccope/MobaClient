﻿// -------------------------------------------------------------------------------
// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
// -------------------------------------------------------------------------------

using GameFW.Entity.Driver;
using System;
using UnityEngine;
using GameFW.GameMgr;
using UnityEngine.AI;
using GameFW.Entity.Helper;
using GameFW.Core.Msg;
using GameFW.Entity;
using GameFW.Core.Base;
/// <summary>
/// 战士的BT
/// </summary>
public class SoilderAgent : FightAgent
{
    #region Behaviac框架生成的属性
    /// <summary>
    /// 当前的攻击目标
    /// </summary>
    private FightAgent curEnemyTarget = null;
    public void _set_curEnemyTarget(FightAgent value)
    {
        curEnemyTarget = value;
    }
    public FightAgent _get_curEnemyTarget()
    {
        return curEnemyTarget;
    }
    /// <summary>
    /// 当前的状况(优势、劣势、不明)
    /// </summary>
    private Situation curSituation = Situation.Advantage;
    public void _set_curSituation(Situation value)
    {
        curSituation = value;
    }
    public Situation _get_curSituation()
    {
        return curSituation;
    }
    /// <summary>
    /// 当前的动作状态(移动、停下什么也没做、停下了在做某些动作)
    /// </summary>
    private ActionStatus curActionStatus = ActionStatus.StopAndDoingNothing;
    public void _set_curActionStatus(ActionStatus value)
    {
        curActionStatus = value;
    }
    public ActionStatus _get_curActionStatus()
    {
        return curActionStatus;
    }
    /// <summary>
    /// 当前战术攻击目的地
    /// </summary>
    private bVector3 curTacticAtkPos = null;
    public void _set_curTacticAtkPos(bVector3 value)
    {
        curTacticAtkPos = value;
    }
    public bVector3 _get_curTacticAtkPos()
    {
        return curTacticAtkPos;
    }
    /// <summary>
    /// 回家点
    /// </summary>
    private bVector3 homePoint = null;
    public void _set_homePoint(bVector3 value)
    {
        homePoint = value;
    }
    public bVector3 _get_homePoint()
    {
        return homePoint;
    }
    #endregion

    #region 寻找敌人
    /// <summary>
    /// 视野范围
    /// </summary>
    public float EyeRange { get; set; }

    /// <summary>
    /// 得到最近的敌人
    /// </summary>
    /// <returns></returns>
    public FightAgent GetNearestEnemy()
    {
        GameObject nearestEnemy = MgrCenter.EntityMgr.GetNearestEnemy(Id, transform.position, EyeRange, gameObject.layer);

        return nearestEnemy == null ? null : nearestEnemy.GetComponent<FightAgent>();
    }
    #endregion

    #region 发呆
    /// <summary>
    /// 请求发呆
    /// </summary>
    /// <returns></returns>
    public behaviac.EBTStatus Idle()
    {
        if (curActionStatus != ActionStatus.StopAndDoingNothing)
        {
            curActionStatus = ActionStatus.StopAndDoingNothing;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgInt((ushort)SoilderFightEvent.SoilderIdleClientRequest, this.Id));
        }
        return behaviac.EBTStatus.BT_SUCCESS;
    }
    #endregion

    #region 移动相关

    private Vector3 curTargetPos;//当前目标位置
    /// <summary>
    /// 向一个位置移动,主动移动放在agent里,driver只负责位置同步
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
	public behaviac.EBTStatus MoveTo(bVector3 targetPos)
    {
        if (curActionStatus != ActionStatus.Moving)
        {
            curActionStatus = ActionStatus.Moving;
            curTargetPos = targetPos.UnityVec();
            MgrCenter.Instance.SendMsg(Msgs.GetMsgMoveRequest((ushort)MoveEvent.MoveClientRequest, Id, transform.position, curTargetPos));
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }
    /// <summary>
    /// 请求向一个目标移动
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
	public behaviac.EBTStatus MoveToTarget(FightAgent target)
    {
        if (curActionStatus != ActionStatus.Moving)
        {
            curActionStatus = ActionStatus.Moving;
            curTargetPos = target.transform.position;
            MgrCenter.Instance.SendMsg(Msgs.GetMsgMoveRequest((ushort)MoveEvent.MoveClientRequest, Id, transform.position, curTargetPos));
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }

    private Vector3 targetSmall;//服务器返回的小的移动目标点
    public Vector3 TargetSmall { get { return targetSmall; } set { this.targetSmall = value; } }
    private float Speed { get; set; }//移动速度
    private NavMeshAgent navMeshAgent;//主机(客户端)负责移动的导航代理
    private bool smallMoveAnswered;//是否已经收到并使用了服务器的small move包

    /// <summary>
    /// 向一个小目标移动,这个小目标是server返回的
    /// </summary>
    /// <param name="smallTarget"></param>
    public void MoveToSmallTarget()
    {
        smallMoveAnswered = false;
        this.navMeshAgent.SetDestination(targetSmall);
    }

    private float lastPosSyncTimestamp;//最后发送同步包时的时间
    private Vector3 lastSendPos;//最后发送的位置
    private Vector3 lastSendDir;//最后发送的方向
    private float tmpDistance;//当前位置和预测位置的距离

    private void FixedUpdate()
    {
        if (curActionStatus == ActionStatus.Moving)
        {
            tmpDistance = DistanceBetweenCurPosAndPredict(transform.position, lastSendPos, lastSendDir, Time.time - lastPosSyncTimestamp, Speed);
            //↓当[当前移动位置]超出[服务器预测位置]时提前发送同步包, 或者是到了同步时间,按定时同步频率发送
            if (tmpDistance > Speed * 1.0f || Time.time >= lastPosSyncTimestamp + GameRuntimeData.PosSyncSecond)
            {
                lastSendDir = transform.eulerAngles;
                lastSendPos = transform.position;
                lastPosSyncTimestamp = Time.time;
                MgrCenter.Instance.SendMsg(Msgs.GetMsgPosSync((ushort)MoveEvent.PosSyncClientRequest, Id, lastSendPos, lastSendDir, DateTime.Now.Ticks));
            }

            //↓如果已经到达小目标点
            if ((!smallMoveAnswered) && Vector3.Distance(targetSmall, transform.position) <= this.Speed * GameRuntimeData.GetDelaySecond * 2.5f)
            {
                //↓但没有到达最终目标点，继续发送请求位置包
                if (Vector3.Distance(curTargetPos, transform.position) > this.Speed * GameRuntimeData.GetDelaySecond * 2.5f)
                {
                    MgrCenter.Instance.SendMsg(Msgs.GetMsgMoveRequest((ushort)MoveEvent.MoveClientRequest, Id, transform.position, curTargetPos));
                }
                smallMoveAnswered = true;
            }
        }
    }

    /// <summary>
    /// 当前位置和推测位置的距离
    /// </summary>
    /// <param name="curPos"></param>
    /// <param name="lastSendPos"></param>
    /// <param name="lastSendDir"></param>
    /// <param name="timeEscaped"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    private float DistanceBetweenCurPosAndPredict(Vector3 curPos, Vector3 lastSendPos, Vector3 lastSendDir, float timeEscaped, float speed)
    {
        return Vector3.Distance(curPos, MoveHelper.GetFuturePos(lastSendPos, lastSendDir, timeEscaped, speed));
    }
    #endregion

    #region 攻击相关
    /// <summary>
    /// 发送攻击请求
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public behaviac.EBTStatus Attack(FightAgent target)
    {
        if (target == null)
        {
            return behaviac.EBTStatus.BT_FAILURE;
        }
        if (target.IsDead())
        {
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        if (curActionStatus != ActionStatus.StopAndDoingSomething)
        {
            curActionStatus = ActionStatus.StopAndDoingSomething;

            //开启一个协程计算时间处理攻击？ 还是用动画事件处理攻击？
            MgrCenter.Instance.SendMsg(Msgs.GetMsgAtkRequest((ushort)SoilderFightEvent.SoilderAtkClientRequest, Id, target.Id));
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }

    public float AtkRange { get; set; }//攻击范围
    /// <summary>
    /// 目标是否在攻击范围内
    /// </summary>
    /// <param name="enemyTarget"></param>
    /// <returns></returns>
    public bool TestEnemyInAtkRange(FightAgent enemyTarget)
    {
        if (enemyTarget != null && enemyTarget.transform != null)
            return Vector3.Distance(transform.position, enemyTarget.transform.position) <= AtkRange;

        return false;
    }

    /// <summary>
    /// 是否有A向某地的命令
    /// </summary>
    /// <returns></returns>
    public bool TestHasAtkToPosCommand()
    {
        return curTacticAtkPos != null && Vector3.Distance(transform.position, curTacticAtkPos.UnityVec()) > 1e-3;
    }
    /// <summary>
    /// 视野范围内是否有敌人
    /// </summary>
    /// <returns></returns>
    public bool TestHasEnemyInEyeRange()
    {
        GameObject nearestEnemy = MgrCenter.EntityMgr.GetNearestEnemy(Id, transform.position, EyeRange, gameObject.layer);

        return nearestEnemy != null;
    }
    #endregion

    #region BT初始化和Agent初始化
    /// <summary>
    /// 初始化BT
    /// </summary>
    /// <returns></returns>
    protected override bool InitPlayer()
    {
        bool bRet = this.btload("SoilderBT");
        if (bRet)
        {
            this.btsetcurrent("SoilderBT");
        }

        return bRet;
    }

    private SoilderDriver soilderDriver;//Agent对应的驱动器

    /// <summary>
    /// 初始化Agent
    /// </summary>
    public override void Initial()
    {
        base.Initial();

        soilderDriver = gameObject.GetComponent<SoilderDriver>();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (soilderDriver == null)
            Debug.LogError("soilder driver not exited.");
        else
        {
            this.Id = soilderDriver.Id;
            this.AtkRange = soilderDriver.AtkRange;
            this.EyeRange = soilderDriver.EyeRange;
            this.Speed = soilderDriver.Speed;
        }
    }
    #endregion

}


