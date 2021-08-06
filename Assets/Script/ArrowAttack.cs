using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 弓箭攻击
 */
public class ArrowAttack : MonoBehaviour
{
    public float speed; //弓箭速度
    public Slider enemyslider; //敌人血量条
    public GameObject enemyHP; //敌人血量显示文字
    GameObject arrow; //用于生成弓箭
    private static float eHPmax; //敌人最大血量
    public static float eHP; //敌人目前血量
    private List<Arrow> arrows = new List<Arrow>();//保存弓箭对象

    // Start is called before the first frame update
    void Start()
    {
        eHPmax = 2000.0f;
        eHP = 2000.0f;
        enemyHP.GetComponent<Text>().text = eHP.ToString();
        speed = 50f;
    }

    // Update is called once per frame
    void Update()
    {
        //弓箭攻击到敌人，改变敌人血量显示，销毁弓箭
        if (Arrowcol.flag)
        {
            eHP -= Readcsv.army.Atk;
            enemyslider.value = eHP / eHPmax;
            enemyHP.GetComponent<Text>().text = eHP.ToString();
            if (eHP <= 0)
            {
                enemyslider.value = 0;
                enemyHP.GetComponent<Text>().text = "0";
                Destroy(GameObject.FindWithTag("enemy"));
                foreach (Arrow arr in arrows)
                {
                    Destroy(arr.gameObject);
                }
                
            }
            
            Destroy(arrows[0].gameObject);
            arrows.Remove(arrows[0]);
            
            Arrowcol.flag = false;
        }
        
        //弓箭对象存在，让弓箭移动
        if (arrow)
        {
            arrowMove();
        }
    }
    
    //实例化弓箭
    public void attack()
    {
        //敌人血量小于0时，不做攻击
        if (eHP <= 0)
        {
            return ;
        }
        arrow = Instantiate(Resources.Load("Prefabs/IceArrow"), 
            transform.position + new Vector3(3, 6, 6), transform.rotation) as GameObject;
        
        //将弓箭加入弓箭list
        Arrow arr = new Arrow();
        arr.gameObject = arrow;
        arr.point = arrow.transform.position;
        arr.speed = speed;
        arrows.Add(arr);
        
        //实例化弓箭后，弓箭手状态改为闲置
        Animator animator = GameObject.FindWithTag("army").GetComponent<Animator>();
        animator.SetBool("isAttack", false);
        animator.SetBool("isStop", true);
        animator.SetBool("isRun", false);
    }
    
    //弓箭移动
    public void arrowMove()
    {
        foreach (Arrow arr in arrows)
        {
            arr.gameObject.transform.position = Vector3.MoveTowards(arr.point, 
                arr.point + new Vector3(0, 0, 2), Time.deltaTime * arr.speed);
            arr.point = arr.gameObject.transform.position;
        }
    }
    
}
