using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossC : MonoBehaviour
{
    #region 公有变量
    public float HP = 100;//Boss血量
    public BossState bossState;//Boss状态
    public Transform 目标;//Boss攻击目标
    public Animator ani;//Boss动画
    public SkinnedMeshRenderer render;//BOSS皮肤，在BOSS收到攻击时皮肤会变红
    #endregion

    #region 私有变量
    float atkTime;  //BOSS攻击时间
    float atkLastTime = 3.5f;//BOSSS
    #endregion


    #region 枚举
    /// <summary>
    /// BOSS状态机
    /// </summary>
    public enum BossState
    {
        巡逻,
        攻击,
        死亡,
    }
    #endregion



    #region 初始化
    void Awake()
    {
        ani =  GetComponentInChildren<Animator>();//初始化BOSS状态机
    }
    #endregion

    #region 更新
    void Update()
    {
        checkDie();//检查BOSS血量
        switch (bossState)//在BOSS不同状态调用不同状态方法
        {
            case BossState.巡逻:
                findTarget();
                break;
            case BossState.攻击:
                atk();
                break;
            case BossState.死亡:
                die();
                break;
        }
    }
    #endregion

    /// <summary>
    /// 当子弹进入BOSS触发器范围调用方法
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (bossState == BossState.死亡)
        {
            return;
        }
        HP = HP -5;//收到攻击，减少血量
        StartCoroutine(changeColor());//启动协程：收到攻击，改变BOSS皮肤颜色
        this.transform.LookAt(目标);
    }

    /// <summary>
    /// 巡逻状态下查找目标
    /// </summary>
    void findTarget()
    {
        this.transform.Rotate(Vector3.up ,Time.deltaTime *30);//旋转自身查找目标
        Vector3 pos = 目标.position;//要查找的目标的坐标
        pos.y = this.transform.position.y;//将查找目标的高度和自身高度同意
        float angle = Vector3.Angle(this.transform.forward, pos - this.transform.position);//boss自身向前向量与BOSS和目标向量的角度
        if (angle < 15 )//当角度小于15°，说明目标进入BOSS视觉范围
        {
            atkTime = Time.time;// 时间戳 
            bossState = BossState.攻击;//更改BOSS状态为攻击状态
        } 
    } 

    /// <summary>
    /// 攻击状态下攻击目标
    /// </summary>
    void atk()
    {
        ani.SetBool("攻击",true);//设置攻击动画的转化调教为true
        if (Time.time - atkTime > atkLastTime)//如果当前时间减去开始攻击时的时间大于攻击持续时间，说明一次攻击结束，将状态改为巡逻状态
        {
            ani.SetBool("攻击", false);
            bossState = BossState.巡逻;
        }
    }

    /// <summary>
    /// BOSS死亡方法
    /// </summary>
    void die()
    {
        ani.SetBool("死亡" ,true);
    }

    /// <summary>
    /// 检查boss血量，小于0时死亡
    /// </summary>
    void checkDie()
    {
        if (HP <=0)
        {
            bossState = BossState.死亡;
        }
    }

    /// <summary>
    /// BOSS收到攻击时改变皮肤颜色
    /// </summary>
    /// <returns></returns>
    IEnumerator changeColor()
    {
        render.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        render.material.color = Color.white;
    }
}
