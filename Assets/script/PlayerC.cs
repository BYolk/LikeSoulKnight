using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主角运动脚本
/// 
/// 刚体Rigidbody：
///     1、Use Gravity：勾选表示使用重力
///     2、Constraints：冻结位置坐标或旋转坐标，冻结后该坐标不可用
///     3、velocity：速度，可以通过刚体的速度实现物体的移动
///     
/// 角色控制器CharacterController：角色控制器
///     1、角色控制器和胶囊体碰撞器很像，就是一个胶囊形状的控制器
///     2、Move()方法：不带重力去移动，适合2d运动
///     3、SimpleMove()方法：带重力移动，适合3d具有高低差的运动（用刚体爬坡会很费力）
///     4、角色控制器会自带碰撞检测，带有角色控制器组件的物体相当于自带碰撞器和刚体
///     
/// 
/// 碰撞检测：
///     1、碰撞体碰撞：
///         1、两个相互碰撞的碰撞体都需要带有碰撞器组件
///         2、两个碰撞体其中之一必须要有刚体组件
///         3、如果两个碰撞体其中之一带有角色控制器，那两个碰撞体可以不用有刚体组件，因为角色控制器就已经相当于自带碰撞器和刚体
///     2、触发器碰撞：
///         1、两个相互碰撞的碰撞体都需要带有碰撞器组件
///         2、两个碰撞体其中之一必须带有刚体组件
///         3、两个碰撞体其中之一的碰撞器组件必须勾选is Trigger
/// </summary>
public class PlayerC : MonoBehaviour
{

    #region 变量
    Vector3 newPosition;            //主角移动的新位置
    public float moveSpeed = 5;     //主角移动速度
    Vector3 moveDir;                //主角移动方向
    public bool isSimple;           //角色控制器 CharacterController 的移动方式是否是SimpleMove
    public MoveType moveType;       //运动枚举类型
    RaycastHit hitInfo;             //射线检测
    Ray ray;                        //射线
    bool isShooting;                //是否射击
    public float HP;
    public SkinnedMeshRenderer render;//角色皮肤
    #endregion

    #region 引用
    public Rigidbody rig;           //角色刚体组件
    CharacterController cc;         //角色控制器组件
    Transform model;                //主角游戏对象
    public Animator ani;            //动画管理器组件
    #endregion

    #region 枚举
    public enum MoveType
    {
        刚体,
        角色控制器,
        更新坐标 
    }
    #endregion

    #region 初始化
    void Start()
    {
        HP = 100;
        switch (moveType)
        {
            case MoveType.刚体: //如果移动类型是通过刚体移动，需要先获取玩家的刚体。先判断获取的刚体是否为空，如果为空表示没有刚体，需要手动添加刚体
                rig = GetComponent<Rigidbody>();
                if (rig==null)
                {
                    rig = this.gameObject.AddComponent<Rigidbody>(); 
                }
                rig.constraints = RigidbodyConstraints.FreezeRotation;//冻结刚体的旋转坐标，当玩家受到力时不会发生旋转
                break;
            case MoveType.角色控制器://如果移动类型是通过角色控制器移动，需要获取玩家的移动控制器组件，若为空也需要手动添加移动控制器
                cc = GetComponent<CharacterController>();
                if (cc ==null)
                {
                    cc = this.gameObject.AddComponent<CharacterController>();
                }
                cc.height = 2.04f;//调整角色控制器的高度和中心，让角色控制器嫩更恰好包裹住主角
                cc.center = new Vector3(0,1,0);
                cc.enabled = true;//激活移动控制器
                break; 
        }
        
        model = this.transform.GetChild(0);//获取玩家模型对象：玩家对象是一个空物体，空物体下面的模型才是玩家对象，要移动玩家，移动玩家的模型对象就可以了，所以要获取模型对象
        ani = model.GetComponent<Animator>();//获取玩家的动画管理器组件，用于播放玩家不同状态的动画，如移动动画，射击动画
    }
    #endregion

    #region 更新
    void Update()
    { 
        atk();
        beforeMove();
        move();
        rotateModel(); 
    }
    #endregion

    #region 角色跟随鼠标旋转方法
    /// <summary>
    /// 角色旋转方法：根据鼠标所在方向旋转角色，让角色始终面向鼠标方向
    /// </summary>
    void rotateModel()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);//在主摄像机的鼠标所在位置发射一条射线，将射线引用保存在ray里
        if (Physics.Raycast(ray, out hitInfo, 1000, 1 << 6))//如果这条射线与层级为“6”的游戏对象（场景中设置地板ground的层级为6）发生碰撞，则将射线检测信息保存到hitinfo上，射线检测最大距离为1000
        {
            Vector3 taget = hitInfo.point;//将碰撞到的位置信息保存到玩家将要面向的三维向量上
            taget.y = this.transform.position.y;//因为碰撞的地面的高度y在地板上，玩家不能看向地板，所以要把玩家的高度赋给要看向的方向的高度，让玩家平时过去
            model.LookAt(taget);//让玩家看向目标方向
        }
    }
    #endregion

    #region 角色运动方法
    /// <summary>
    /// 在运动前获取主角下一次运动后的新坐标
    /// 为什么获取新坐标要独立出来一个方法：在很多业务需求中都需要获取玩家下一次运动位置的坐标，将其独立成一个方法更有利于程序的健壮性
    /// 
    /// 获取运动后新坐标的方式：
    ///     1、先获得运动方向：
    ///         1、Input系统获取玩家按键：
    ///             1、Input.GetAxis:
    ///                 1、Input.GetAxis("Horizontal")：当玩家按下A，从0开始往-1递减并返回，当玩家按下D，从0开始往1递增
    ///                 2、Input.GetAxis("Vertical")：当玩家按下S，从0开始往-1递增，当玩家按下W，从0开始往1递增
    ///             2、Input.GetAxisRaw：
    ///                 1、Input.GetAxisRaw("Horizontal")：当玩家按下A，返回-1，当玩家按下D，返回1
    ///                 2、Input.GetAxisRaw("Vertical")：当玩家按下S，返回-1，当玩家按下W，返回1
    ///             3、对于主角的运动：
    ///                 1、使用Input.GetAxisRaw更好，因为直接返回-1或1，在三维向量中是一个常量。
    ///                 2、若使用Input.GetAxis，返回一个递增的量，所以三维向量是一个变量，再乘玩家的位移，导致距离也是一个改变的量，会造成玩家运动加速现象，改变运动方向时也不够平滑
    ///     2、在运动方向上，速度*时间=距离，实现主角运动：
    ///         1、运动位移是一个向量，需要方向，所以需要“运动距离*方向向量”
    ///         2、运动距离 = 速度 * 时间
    ///         3、Time.deltaTime：
    ///             1、增量时间：表示一帧花费的时间
    ///             2、update方法每帧调用一次，好的机器1秒帧数比差的机器1秒帧数要多得多，所以好的机器增量时间小（比如1s运行180帧，增量时间Time.deltaTime为1/180），差的机器增量时间大（比如1s运行60帧，增量时间Time.deltaTime为1/60)
    ///             3、1s的帧数*增量时间=1s（比如好的机器1s帧数为180，增量时间为1/180，两者相乘为1）
    /// </summary>
    void beforeMove()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        newPosition = moveDir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 移动方法：
    ///     1、先获取移动后的新坐标
    ///     2、根据移动枚举判断移动方式
    ///     3、如果移动方向是零向量，说明不移动，则不播放移动动画，让动画状态机转换条件的“移动”变量为false，表示状态不向移动状态转换
    ///     4、如果移动方向向量不是零向量，说明要进行移动，则让动画状态机的“移动”变量为true，让状态转为移动状态，播放移动动画
    ///     5、判断主角的移动方式：
    ///         1、更新坐标方式：将移动后的新坐标赋给玩家当前坐标，实现玩家移动
    ///         2、刚体方式：给玩家添加刚体，刚体具有重量和速度等属性，通过改变刚体的速度velocity，进而实现玩家移动（注意，因为玩家移动后的新坐标newPosition是一个三维向量，它的模很小，所以需要乘以一个系数，系数大小根据实际情况调整
    ///         3、角色控制器方式：
    ///             1、Move()：不带重力移动
    ///             2、SampleMove()：带重力移动
    /// </summary>
    private void move()
    { 
        switch (moveType)
        {
            case MoveType.刚体:
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                if (horizontal != 0 || vertical != 0)
                {
                    ani.SetBool("移动", true);
                    rig.velocity = newPosition * 100;
                }
                else
                {
                    ani.SetBool("移动", false);
                }
                break;
            case MoveType.角色控制器:
                if (isSimple)
                {
                    horizontal = Input.GetAxis("Horizontal");
                    vertical = Input.GetAxis("Vertical");
                    if(horizontal != 0 || vertical != 0)
                    {
                        ani.SetBool("移动", true);
                        cc.SimpleMove(newPosition * 100);
                    }
                    else
                    {
                        ani.SetBool("移动", false);
                    }
                }
                else
                {
                    cc.Move(newPosition);
                } 
                break;
            case MoveType.更新坐标:
                this.transform.position += newPosition;
                break;
        }   
    }
    #endregion


    #region 角色攻击方法
    /// <summary>
    /// 播放动画方法：
    ///     当玩家按下鼠标左键，改变动画状态机的“射击ing”变量为true，更改玩家状态为射击状态，播放射击动画
    ///     当玩家松开鼠标左键，改变动画状态机的“射击ing”变量为false，更改玩家状态为非射击状态，停止播放射击动画
    /// </summary>
    void atk()
    { 
        if (Input.GetMouseButtonDown(0) )
        {
            isShooting = true;
            ani.SetBool("射击ing", true); 
        } 
        if (Input.GetMouseButtonUp(0))
        {
            ani.SetBool("射击ing", false);
            isShooting = false; 
        }
    }
    #endregion

    /// <summary>
    /// 玩家受到攻击
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name != "Cube" && HP >= 0)
        {
            HP = HP - 5;//收到攻击，减少血量
            StartCoroutine(ChangeColor());//启动协程：收到攻击，改变BOSS皮肤颜色
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
    /// 主角受到攻击时改变皮肤颜色
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeColor()
    {
        render.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        render.material.color = Color.white;
    }

    /// <summary>
    /// 播放 0.1s 受伤动画后恢复
    /// </summary>
    /// <returns></returns>
    IEnumerator HurtAnimationComplete()
    {
        yield return new WaitForSeconds(0.1f);
        ani.SetBool("Hurt", false);
    }
}
