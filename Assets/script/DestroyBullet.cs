using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 销毁（回收）对象脚本
/// </summary>
public class DestroyBullet : MonoBehaviour
{
    GameObject obj;          //要销毁（回收）的游戏对象
    Rigidbody rig;          //刚体组件引用
    float activeTime = 3;   //对象存活时间

    #region 不使用对象池销毁对象
    //float desTime = 3;
    //void Start()
    //{
    //    Destroy(this.gameObject, desTime);

    //}
    private void Awake()
    {
        //obj = this.gameObject;
        //rig = GetComponent<Rigidbody>();
    }
    #endregion



    #region 更新
    /// <summary>
    /// 倒计时子弹存活时间，时间小于0时回收子弹对象
    /// </summary>
    void Update()
    {
        activeTime -= Time.deltaTime;
        if (activeTime < 0)
        {
            //   Destroy(this.gameObject);
            BulletPoolMnager.intance.recoverBullet(obj);
        }
    }
    #endregion

    /// <summary>
    /// 使用子弹对象后，该子弹对象就
    /// </summary>
    private void OnDisable()
    {
        rig.velocity = Vector3.zero;
    }

    /// <summary>
    /// 激活子弹对象：从子弹对象池中取出子弹对象，激活该子弹对象
    /// </summary>
    private void OnEnable()
    {
        if (rig == null)
        {
            obj = this.gameObject;
            rig = GetComponent<Rigidbody>();
        }
        activeTime = 3;
    }
}
