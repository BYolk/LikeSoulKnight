using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossC : MonoBehaviour
{
    #region ���б���
    public float HP = 100;//BossѪ��
    public BossState bossState;//Boss״̬
    public Transform Ŀ��;//Boss����Ŀ��
    public Animator ani;//Boss����
    public SkinnedMeshRenderer render;//BOSSƤ������BOSS�յ�����ʱƤ������
    #endregion

    #region ˽�б���
    float atkTime;  //BOSS����ʱ��
    float atkLastTime = 3.5f;//BOSSS
    #endregion


    #region ö��
    /// <summary>
    /// BOSS״̬��
    /// </summary>
    public enum BossState
    {
        Ѳ��,
        ����,
        ����,
    }
    #endregion



    #region ��ʼ��
    void Awake()
    {
        ani =  GetComponentInChildren<Animator>();//��ʼ��BOSS״̬��
    }
    #endregion

    #region ����
    void Update()
    {
        checkDie();//���BOSSѪ��
        switch (bossState)//��BOSS��ͬ״̬���ò�ͬ״̬����
        {
            case BossState.Ѳ��:
                findTarget();
                break;
            case BossState.����:
                atk();
                break;
            case BossState.����:
                die();
                break;
        }
    }
    #endregion

    /// <summary>
    /// ���ӵ�����BOSS��������Χ���÷���
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (bossState == BossState.����)
        {
            return;
        }
        HP = HP -5;//�յ�����������Ѫ��
        StartCoroutine(changeColor());//����Э�̣��յ��������ı�BOSSƤ����ɫ
        this.transform.LookAt(Ŀ��);
    }

    /// <summary>
    /// Ѳ��״̬�²���Ŀ��
    /// </summary>
    void findTarget()
    {
        this.transform.Rotate(Vector3.up ,Time.deltaTime *30);//��ת�������Ŀ��
        Vector3 pos = Ŀ��.position;//Ҫ���ҵ�Ŀ�������
        pos.y = this.transform.position.y;//������Ŀ��ĸ߶Ⱥ�����߶�ͬ��
        float angle = Vector3.Angle(this.transform.forward, pos - this.transform.position);//boss������ǰ������BOSS��Ŀ�������ĽǶ�
        if (angle < 15 )//���Ƕ�С��15�㣬˵��Ŀ�����BOSS�Ӿ���Χ
        {
            atkTime = Time.time;// ʱ��� 
            bossState = BossState.����;//����BOSS״̬Ϊ����״̬
        } 
    } 

    /// <summary>
    /// ����״̬�¹���Ŀ��
    /// </summary>
    void atk()
    {
        ani.SetBool("����",true);//���ù���������ת������Ϊtrue
        if (Time.time - atkTime > atkLastTime)//�����ǰʱ���ȥ��ʼ����ʱ��ʱ����ڹ�������ʱ�䣬˵��һ�ι�����������״̬��ΪѲ��״̬
        {
            ani.SetBool("����", false);
            bossState = BossState.Ѳ��;
        }
    }

    /// <summary>
    /// BOSS��������
    /// </summary>
    void die()
    {
        ani.SetBool("����" ,true);
    }

    /// <summary>
    /// ���bossѪ����С��0ʱ����
    /// </summary>
    void checkDie()
    {
        if (HP <=0)
        {
            bossState = BossState.����;
        }
    }

    /// <summary>
    /// BOSS�յ�����ʱ�ı�Ƥ����ɫ
    /// </summary>
    /// <returns></returns>
    IEnumerator changeColor()
    {
        render.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        render.material.color = Color.white;
    }
}
