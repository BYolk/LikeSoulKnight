using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����¼��ű��������ǵ���������У��������������һ֡ע��˽ű���shoot�������������������ʱ���ڲ����������һ֡�У�����ô˽ű���shoot����
/// </summary>
public class AniEvent : MonoBehaviour
{

    public GameObject bullet_prefab;//�ӵ�Ԥ��������
    public string bulletLayer;//�ӵ��㼶
    public Transform[] shootpos;//�ӵ����λ�����ã�ǹ��λ�����ã����飨������ǹ�����⣩
    public float speed = 500;//�ӵ��ٶ�
    public void shoot()
    {
        for (int i = 0; i <shootpos.Length;i++)
        {
            GameObject bullet = BulletPoolMnager.intance.getBullet();//�ڶ�����л�ȡ�ӵ����󣬽��ӵ�����ֵ����Ϸ��������
            bullet.transform.position = shootpos[i].position;// this.transform.position + this.transform.forward + Vector3.up * 1.2f;//��ȡ���ش˽ű�����Ϸ���󣨼����ģ�ͣ���λ�ã��ڴ�λ������ǰ1������1������ʵ���������������λ�ã�
            bullet.GetComponent<Rigidbody>().AddForce(shootpos[i].forward * speed);//��ȡ�ӵ�������������Ϊ�����һ�������������ӵ��˶����ٶ�Ϊ500
            bullet.layer = LayerMask.NameToLayer(bulletLayer);//�����ӵ��㼶���ӵ�ֻ��ĳ���ַ�����ײ��
        }
    }
}
