using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 弓箭手控制类，进行弓箭手的血量修改、动作切换和攻击操作
 */
public class ArcherController : MonoBehaviour
{
    public Animator archerAnimator; //弓箭手的动画
    public Slider archerHPSlider; //弓箭手血量条
    public Text archerHPText; //弓箭手血量显示数字
    
    public EnemyController enemyController; //敌人控制类
    public ArrowController arrowController; //弓箭控制类
    
    public ArmyModel armyData; //保存读取的csv数据
    private float shootSpeed; //弓箭手攻速
    public int atk; //弓箭手攻击力
    private float archerMaxHP; //弓箭手最大血量
    [HideInInspector]
    public float archerHP; //弓箭手目前血量
    private float timer; //计时器，用于弓箭手攻击速度的限制

    private void Awake()
    {
        armyData = CsvReader.Readcsv();
    }

    void Start()
    {
        shootSpeed = armyData.ShootSpeed;
        timer = shootSpeed;
        atk = armyData.Atk;
        archerMaxHP = armyData.MaxHp;
        archerHP = archerMaxHP;
        ModifyArcherHP(0);
    }
    
    void Update()
    {
        SwitchAction();
    }
    
    //修改弓箭手血量和血量条
    private void ModifyArcherHP(int addArcherHP)
    {
        archerHP += addArcherHP;
        if (archerHP <= 0)
        {
            archerHP = 0;
        }
        archerHPText.text = archerHP.ToString();
        archerHPSlider.value = archerHP / archerMaxHP;
    }
    
    //弓箭手动作切换
    private void SwitchAction()
    {
        //A键攻击，敌人血量要大于0且计时器大于攻速才进行攻击
        if (Input.GetKeyDown(KeyCode.A) && enemyController.enemyHP > 0 && timer > shootSpeed) 
        {
            timer = 0.0f;
            archerAnimator.SetTrigger("isAttack");
        }
        //R键跑步
        else if (Input.GetKeyDown(KeyCode.R)) 
        {
            archerAnimator.SetTrigger("isRun");
        }
        //I键闲置
        else if (Input.GetKeyDown(KeyCode.I)) 
        {
            archerAnimator.SetTrigger("isStop");
        }
        timer += Time.deltaTime;
    }
    
    //攻击事件，绑定在攻击动画后
    private void Attack()
    {
        arrowController.CreateArrow();
        
        //实例化弓箭后，弓箭手状态改为闲置
        archerAnimator.ResetTrigger("isAttack");
        archerAnimator.SetTrigger("isStop");
    }
}
