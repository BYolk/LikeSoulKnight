using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹的对象池：
///     1、为什么要建立子弹的对象池？
///         1、如果不用子弹对象池，那么需要频繁创建子弹对象，子弹对象如果不销毁，对性能消耗将无限量增大。
///         2、如果频繁销毁子弹对象，就需要频繁调用垃圾回收器GC，频繁调用GC对性能消耗也很大
///         3、所以有种想法，能不能不要频繁创建销毁子弹对象，而是一次性创建若干个子弹对象，在用过子弹对象后，将子弹对象回收起来，用于下次再用，就像一次性筷子和家用筷子
///     2、根据射击速度调整子弹对象池的子弹对象数量，用一个队列来存放子弹对象（从队列尾部回收子弹对象，从队列头部使用子弹对象）
/// </summary>
public class BulletPoolMnager : MonoBehaviour
{
    public static BulletPoolMnager intance;  //创建单例

    public GameObject bullet_prefab;    //创建子弹对象引用
    Queue<GameObject> bulletPool;   //创建存放子弹对象的队列
    int poolInitLength = 20;   //定义子弹对象池中的子弹对象数量





    #region 初始化
    private void Awake()
    {
        intance = this;//给单例赋值
    }


    void Start()
    {
        // 初始化对象池
        bulletPool = new Queue<GameObject>();//给子弹对象池引用赋值
        GameObject bullet = null;//给创建空的子弹对象引用
        for (int i = 0; i < poolInitLength;i++)
        {
            bullet = Instantiate(bullet_prefab);//实例化一个子弹对象，将其赋值给空的子弹对象应用
            bullet.SetActive(false);//先将子弹对象禁用，在射击时再激活
            bulletPool.Enqueue(bullet);//将子弹对象添加到队列中，即子弹对象池中
            
        }
    }
    #endregion


    #region 使用与回收子弹对象
    /// <summary>
    /// 获取子弹对象
    /// </summary>
    /// <returns>返回一个子弹预制体对象</returns>
    public GameObject getBullet() // 获取子弹
    {
        if (bulletPool.Count > 0)//如果子弹对象池的数量大于0，说明够用
        {
            GameObject bullet = bulletPool.Dequeue();//从子弹对象池中取出一个子弹对象
            bullet.SetActive(true);//将该子弹对象激活
            return bullet;//返回子弹对象
        }
        else//代码走这里说明子弹对象池中已经没有子弹对象了，需要手动创建一个新的子弹对象
        {
            return Instantiate(bullet_prefab);
        }
    }

    /// <summary>
    /// 回收子弹对象方法
    /// </summary>
    /// <param name="bullet">需要回收的子弹对象</param>
    public void recoverBullet(GameObject bullet) 
    {
        bullet.SetActive(false);//将子弹对象禁用
        bulletPool.Enqueue(bullet); //将子弹对象回收到队列（对象池）中
    }
    #endregion
}
