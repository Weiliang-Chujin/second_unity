using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 敌人类，保存敌人属性，并修改敌人血量和销毁敌人
 */
public class Enemy : MonoBehaviour
{
    public Animator archerAnimator; //弓箭手的动画
    public GameObject enemy; //敌人游戏对象
    public Slider enemyHPSlider; //敌人血量条
    public Text enemyHPText; //敌人血量显示数字
    public CsvRead csvRead; //读取csv文件类
    public ArrowAction arrowAction; //弓箭动作类
    public Archer archer; //弓箭手类
    
    private float enemyMaxHP; //敌人最大血量
    [HideInInspector]
    public float enemyHP; //敌人目前血量
    
     void Start()
     {
         enemyMaxHP = csvRead.armyData.MaxHp;
         enemyHP = enemyMaxHP;
         enemyHPSlider.value = enemyHP / enemyMaxHP;
         ModifyEnemyHP(0);
     }
     
    //修改敌人血量和血量条
    public void ModifyEnemyHP(int addEnemyHP)
    {
        enemyHP += addEnemyHP;
        
        //敌人血量小于等于0时，血量改为0，销毁敌人
        if (enemyHP <= 0)
        {
            enemyHP = 0;
            DestroyEnemy();
        }
        enemyHPText.text = enemyHP.ToString();
        enemyHPSlider.value = enemyHP / enemyMaxHP;
    }
    
    //销毁敌人，并销毁所有弓箭，弓箭手状态切换为闲置状态
    public void DestroyEnemy()
    {
        Destroy(enemy);
        arrowAction.DestroyArrows();
        
        archerAnimator.ResetTrigger("isAttack");
        archerAnimator.SetTrigger("isStop");
    }
    
    //判断弓箭和敌人是否发生触发，发生触发，将那个弓箭销毁，并修改敌人血量
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "arrow")
        {
            arrowAction.DestroyArrow();
            ModifyEnemyHP(-archer.atk);
        }
    }
}
