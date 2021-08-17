using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 敌人控制类，修改敌人血量、销毁敌人和判断弓箭与敌人触发
 */
public class EnemyController : MonoBehaviour
{
    public Animator archerAnimator; //弓箭手的动画
    public GameObject enemy; //敌人游戏对象
    public Slider enemyHPSlider; //敌人血量条
    public Text enemyHPText; //敌人血量显示数字
    
    public CsvReader csvReader; //读取csv文件类
    public ArrowController arrowController; //弓箭控制类
    public ArcherController archerController; //弓箭手控制类
    
    private float enemyMaxHP; //敌人最大血量
    [HideInInspector]
    public float enemyHP; //敌人目前血量
    
     void Start()
     {
         enemyMaxHP = csvReader.armyData.MaxHp;
         enemyHP = enemyMaxHP;
         enemyHPSlider.value = enemyHP / enemyMaxHP;
         ModifyEnemyHP(0);
     }
     
    //修改敌人血量和血量条
    private void ModifyEnemyHP(int addEnemyHP)
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
    private void DestroyEnemy()
    {
        Destroy(enemy);
        arrowController.DestroyArrows();
        
        archerAnimator.ResetTrigger("isAttack");
        archerAnimator.SetTrigger("isStop");
    }
    
    //判断弓箭和敌人是否发生触发，发生触发，将那个弓箭销毁，并修改敌人血量
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "arrow")
        {
            arrowController.DestroyArrow();
            ModifyEnemyHP(-archerController.atk);
        }
    }
}
