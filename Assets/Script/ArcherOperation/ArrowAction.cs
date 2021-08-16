using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 弓箭动作类, 包括生成弓箭，弓箭移动，弓箭销毁操作
 */
public class ArrowAction : MonoBehaviour
{
    public Enemy enemy; //敌人类 
    public GameObject arrowPrefab; //弓箭预制体
    
    private List<Arrow> arrows = new List<Arrow>();//保存弓箭对象
    
    void Update()
    {
        //弓箭对象存在，让弓箭移动
        if (arrows.Count > 0)
        {
            MoveArrow();
        }
    }

    //实例化弓箭出来进行攻击
    public void CreateArrow()
    {
        //敌人血量小于等于0时，不生成弓箭
        if (enemy.enemyHP <= 0)
        {
            return;
        }
        
        GameObject arrowObj = Instantiate(arrowPrefab, transform.position, transform.rotation);
        
        //将弓箭加入弓箭list
        Arrow arrow = new Arrow();
        arrow.gameObject = arrowObj;
        arrow.point = arrowObj.transform.position;
        arrow.speed = 50f;
        arrow.moveDistance = 2f;
        arrows.Add(arrow);
    }
    
    //移动弓箭，让其z坐标移动
    public void MoveArrow()
    {
        foreach (Arrow arrow in arrows)
        {
            arrow.gameObject.transform.position = Vector3.MoveTowards(arrow.point, 
                arrow.point + new Vector3(0, 0, arrow.moveDistance), Time.deltaTime * arrow.speed);
            arrow.point = arrow.gameObject.transform.position;
        }
    }
    
    //销毁单个弓箭对象
    public void DestroyArrow()
    {
        Destroy(arrows[0].gameObject);
        arrows.Remove(arrows[0]);
    }
    
    //销毁全部弓箭对象
    public void DestroyArrows()
    {
        foreach (Arrow arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }
        arrows.Clear();
    }
}
