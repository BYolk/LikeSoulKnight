using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����˶��ű�
/// 
/// ����Rigidbody��
///     1��Use Gravity����ѡ��ʾʹ������
///     2��Constraints������λ���������ת���꣬���������겻����
///     3��velocity���ٶȣ�����ͨ��������ٶ�ʵ��������ƶ�
///     
/// ��ɫ������CharacterController����ɫ������
///     1����ɫ�������ͽ�������ײ�����񣬾���һ��������״�Ŀ�����
///     2��Move()��������������ȥ�ƶ����ʺ�2d�˶�
///     3��SimpleMove()�������������ƶ����ʺ�3d���иߵͲ���˶����ø������»�ܷ�����
///     4����ɫ���������Դ���ײ��⣬���н�ɫ����������������൱���Դ���ײ���͸���
///     
/// 
/// ��ײ��⣺
///     1����ײ����ײ��
///         1�������໥��ײ����ײ�嶼��Ҫ������ײ�����
///         2��������ײ������֮һ����Ҫ�и������
///         3�����������ײ������֮һ���н�ɫ����������������ײ����Բ����и����������Ϊ��ɫ���������Ѿ��൱���Դ���ײ���͸���
///     2����������ײ��
///         1�������໥��ײ����ײ�嶼��Ҫ������ײ�����
///         2��������ײ������֮һ������и������
///         3��������ײ������֮һ����ײ��������빴ѡis Trigger
/// </summary>
public class PlayerC : MonoBehaviour
{

    #region ����
    Vector3 newPosition;            //�����ƶ�����λ��
    public float moveSpeed = 5;     //�����ƶ��ٶ�
    Vector3 moveDir;                //�����ƶ�����
    public bool isSimple;           //��ɫ������ CharacterController ���ƶ���ʽ�Ƿ���SimpleMove
    public MoveType moveType;       //�˶�ö������
    RaycastHit hitInfo;             //���߼��
    Ray ray;                        //����
    bool isShooting;                //�Ƿ����
    public float HP;
    public SkinnedMeshRenderer render;//��ɫƤ��
    #endregion

    #region ����
    public Rigidbody rig;           //��ɫ�������
    CharacterController cc;         //��ɫ���������
    Transform model;                //������Ϸ����
    public Animator ani;            //�������������
    #endregion

    #region ö��
    public enum MoveType
    {
        ����,
        ��ɫ������,
        �������� 
    }
    #endregion

    #region ��ʼ��
    void Start()
    {
        HP = 100;
        switch (moveType)
        {
            case MoveType.����: //����ƶ�������ͨ�������ƶ�����Ҫ�Ȼ�ȡ��ҵĸ��塣���жϻ�ȡ�ĸ����Ƿ�Ϊ�գ����Ϊ�ձ�ʾû�и��壬��Ҫ�ֶ���Ӹ���
                rig = GetComponent<Rigidbody>();
                if (rig==null)
                {
                    rig = this.gameObject.AddComponent<Rigidbody>(); 
                }
                rig.constraints = RigidbodyConstraints.FreezeRotation;//����������ת���꣬������ܵ���ʱ���ᷢ����ת
                break;
            case MoveType.��ɫ������://����ƶ�������ͨ����ɫ�������ƶ�����Ҫ��ȡ��ҵ��ƶ��������������Ϊ��Ҳ��Ҫ�ֶ�����ƶ�������
                cc = GetComponent<CharacterController>();
                if (cc ==null)
                {
                    cc = this.gameObject.AddComponent<CharacterController>();
                }
                cc.height = 2.04f;//������ɫ�������ĸ߶Ⱥ����ģ��ý�ɫ�������۸�ǡ�ð���ס����
                cc.center = new Vector3(0,1,0);
                cc.enabled = true;//�����ƶ�������
                break; 
        }
        
        model = this.transform.GetChild(0);//��ȡ���ģ�Ͷ�����Ҷ�����һ�������壬�����������ģ�Ͳ�����Ҷ���Ҫ�ƶ���ң��ƶ���ҵ�ģ�Ͷ���Ϳ����ˣ�����Ҫ��ȡģ�Ͷ���
        ani = model.GetComponent<Animator>();//��ȡ��ҵĶ�����������������ڲ�����Ҳ�ͬ״̬�Ķ��������ƶ��������������
    }
    #endregion

    #region ����
    void Update()
    { 
        atk();
        beforeMove();
        move();
        rotateModel(); 
    }
    #endregion

    #region ��ɫ���������ת����
    /// <summary>
    /// ��ɫ��ת����������������ڷ�����ת��ɫ���ý�ɫʼ��������귽��
    /// </summary>
    void rotateModel()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);//������������������λ�÷���һ�����ߣ����������ñ�����ray��
        if (Physics.Raycast(ray, out hitInfo, 1000, 1 << 6))//�������������㼶Ϊ��6������Ϸ���󣨳��������õذ�ground�Ĳ㼶Ϊ6��������ײ�������߼����Ϣ���浽hitinfo�ϣ����߼��������Ϊ1000
        {
            Vector3 taget = hitInfo.point;//����ײ����λ����Ϣ���浽��ҽ�Ҫ�������ά������
            taget.y = this.transform.position.y;//��Ϊ��ײ�ĵ���ĸ߶�y�ڵذ��ϣ���Ҳ��ܿ���ذ壬����Ҫ����ҵĸ߶ȸ���Ҫ����ķ���ĸ߶ȣ������ƽʱ��ȥ
            model.LookAt(taget);//����ҿ���Ŀ�귽��
        }
    }
    #endregion

    #region ��ɫ�˶�����
    /// <summary>
    /// ���˶�ǰ��ȡ������һ���˶����������
    /// Ϊʲô��ȡ������Ҫ��������һ���������ںܶ�ҵ�������ж���Ҫ��ȡ�����һ���˶�λ�õ����꣬���������һ�������������ڳ���Ľ�׳��
    /// 
    /// ��ȡ�˶���������ķ�ʽ��
    ///     1���Ȼ���˶�����
    ///         1��Inputϵͳ��ȡ��Ұ�����
    ///             1��Input.GetAxis:
    ///                 1��Input.GetAxis("Horizontal")������Ұ���A����0��ʼ��-1�ݼ������أ�����Ұ���D����0��ʼ��1����
    ///                 2��Input.GetAxis("Vertical")������Ұ���S����0��ʼ��-1����������Ұ���W����0��ʼ��1����
    ///             2��Input.GetAxisRaw��
    ///                 1��Input.GetAxisRaw("Horizontal")������Ұ���A������-1������Ұ���D������1
    ///                 2��Input.GetAxisRaw("Vertical")������Ұ���S������-1������Ұ���W������1
    ///             3���������ǵ��˶���
    ///                 1��ʹ��Input.GetAxisRaw���ã���Ϊֱ�ӷ���-1��1������ά��������һ��������
    ///                 2����ʹ��Input.GetAxis������һ������������������ά������һ���������ٳ���ҵ�λ�ƣ����¾���Ҳ��һ���ı���������������˶��������󣬸ı��˶�����ʱҲ����ƽ��
    ///     2�����˶������ϣ��ٶ�*ʱ��=���룬ʵ�������˶���
    ///         1���˶�λ����һ����������Ҫ����������Ҫ���˶�����*����������
    ///         2���˶����� = �ٶ� * ʱ��
    ///         3��Time.deltaTime��
    ///             1������ʱ�䣺��ʾһ֡���ѵ�ʱ��
    ///             2��update����ÿ֡����һ�Σ��õĻ���1��֡���Ȳ�Ļ���1��֡��Ҫ��ö࣬���ԺõĻ�������ʱ��С������1s����180֡������ʱ��Time.deltaTimeΪ1/180������Ļ�������ʱ��󣨱���1s����60֡������ʱ��Time.deltaTimeΪ1/60)
    ///             3��1s��֡��*����ʱ��=1s������õĻ���1s֡��Ϊ180������ʱ��Ϊ1/180���������Ϊ1��
    /// </summary>
    void beforeMove()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        newPosition = moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �ƶ�������
    ///     1���Ȼ�ȡ�ƶ����������
    ///     2�������ƶ�ö���ж��ƶ���ʽ
    ///     3������ƶ���������������˵�����ƶ����򲻲����ƶ��������ö���״̬��ת�������ġ��ƶ�������Ϊfalse����ʾ״̬�����ƶ�״̬ת��
    ///     4������ƶ���������������������˵��Ҫ�����ƶ������ö���״̬���ġ��ƶ�������Ϊtrue����״̬תΪ�ƶ�״̬�������ƶ�����
    ///     5���ж����ǵ��ƶ���ʽ��
    ///         1���������귽ʽ�����ƶ���������긳����ҵ�ǰ���꣬ʵ������ƶ�
    ///         2�����巽ʽ���������Ӹ��壬��������������ٶȵ����ԣ�ͨ���ı������ٶ�velocity������ʵ������ƶ���ע�⣬��Ϊ����ƶ����������newPosition��һ����ά����������ģ��С��������Ҫ����һ��ϵ����ϵ����С����ʵ���������
    ///         3����ɫ��������ʽ��
    ///             1��Move()�����������ƶ�
    ///             2��SampleMove()���������ƶ�
    /// </summary>
    private void move()
    { 
        switch (moveType)
        {
            case MoveType.����:
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                if (horizontal != 0 || vertical != 0)
                {
                    ani.SetBool("�ƶ�", true);
                    rig.velocity = newPosition * 100;
                }
                else
                {
                    ani.SetBool("�ƶ�", false);
                }
                break;
            case MoveType.��ɫ������:
                if (isSimple)
                {
                    horizontal = Input.GetAxis("Horizontal");
                    vertical = Input.GetAxis("Vertical");
                    if(horizontal != 0 || vertical != 0)
                    {
                        ani.SetBool("�ƶ�", true);
                        cc.SimpleMove(newPosition * 100);
                    }
                    else
                    {
                        ani.SetBool("�ƶ�", false);
                    }
                }
                else
                {
                    cc.Move(newPosition);
                } 
                break;
            case MoveType.��������:
                this.transform.position += newPosition;
                break;
        }   
    }
    #endregion


    #region ��ɫ��������
    /// <summary>
    /// ���Ŷ���������
    ///     ����Ұ������������ı䶯��״̬���ġ����ing������Ϊtrue���������״̬Ϊ���״̬�������������
    ///     ������ɿ����������ı䶯��״̬���ġ����ing������Ϊfalse���������״̬Ϊ�����״̬��ֹͣ�����������
    /// </summary>
    void atk()
    { 
        if (Input.GetMouseButtonDown(0) )
        {
            isShooting = true;
            ani.SetBool("���ing", true); 
        } 
        if (Input.GetMouseButtonUp(0))
        {
            ani.SetBool("���ing", false);
            isShooting = false; 
        }
    }
    #endregion

    /// <summary>
    /// ����ܵ�����
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name != "Cube" && HP >= 0)
        {
            HP = HP - 5;//�յ�����������Ѫ��
            StartCoroutine(ChangeColor());//����Э�̣��յ��������ı�BOSSƤ����ɫ
            ani.SetBool("Hurt", true);
            StartCoroutine(HurtAnimationComplete());
        }
        else if (HP < 0)
        {
            HP = 0;
            ani.SetBool("Sleep", true);
        }
        
    }


    /// <summary>
    /// �����ܵ�����ʱ�ı�Ƥ����ɫ
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeColor()
    {
        render.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        render.material.color = Color.white;
    }

    /// <summary>
    /// ���� 0.1s ���˶�����ָ�
    /// </summary>
    /// <returns></returns>
    IEnumerator HurtAnimationComplete()
    {
        yield return new WaitForSeconds(0.1f);
        ani.SetBool("Hurt", false);
    }
}
