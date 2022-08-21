using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 射击事件脚本：在主角的射击动画中，在主角射击的那一帧注册此脚本的shoot方法，即播放射击动画时，在播放射击的那一帧中，会调用此脚本的shoot方法
/// </summary>
public class AniEvent : MonoBehaviour
{

    public GameObject bullet_prefab;//子弹预制体引用
    public string bulletLayer;//子弹层级
    public Transform[] shootpos;//子弹射击位置引用（枪口位置引用）数组（处理多个枪口问题）
    public float speed = 500;//子弹速度
    public void shoot()
    {
        for (int i = 0; i <shootpos.Length;i++)
        {
            GameObject bullet = BulletPoolMnager.intance.getBullet();//在对象池中获取子弹对象，将子弹对象赋值给游戏对象引用
            bullet.transform.position = shootpos[i].position;// this.transform.position + this.transform.forward + Vector3.up * 1.2f;//获取挂载此脚本的游戏对象（即玩家模型）的位置，在此位置上向前1，向上1（根据实际情况调整到合适位置）
            bullet.GetComponent<Rigidbody>().AddForce(shootpos[i].forward * speed);//获取子弹对象刚体组件，为其加上一个力，力会让子弹运动，速度为500
            bullet.layer = LayerMask.NameToLayer(bulletLayer);//设置子弹层级（子弹只和某部分发生碰撞）
        }
    }
}
