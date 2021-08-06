using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 弓箭手动作切换
 */
public class ArmyAction : MonoBehaviour
{
    public GameObject army; //弓箭手对象
    private float timer; //计时器
    public float attackRateTime; //多少秒攻击一次,攻速

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
        attackRateTime = Readcsv.army.ShootSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        action();
        timer += Time.deltaTime;
    }
    
    //各动作切换
    public void action()
    {
        Animator animator = army.GetComponent<Animator>();
        if (Input.GetKeyDown(KeyCode.A) && timer > attackRateTime && ArrowAttack.eHP > 0) //A键攻击，射箭时间要大于攻速时间，敌人血量要大于0
        {
            timer = 0.0f;
            animator.SetBool("isAttack", true);
            animator.SetBool("isStop", false);
            animator.SetBool("isRun", false);
        }else if (Input.GetKeyDown(KeyCode.R)) //R键跑步
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isStop", false);
            animator.SetBool("isRun", true);
        }else if (Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.A) && timer < attackRateTime)) //I键闲置，射箭时间要小于攻速时间
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isStop", true);
            animator.SetBool("isRun", false);
        }
    }
}
