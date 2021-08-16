using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 弓箭类，保存弓箭数据
 */
public class Arrow
{
    public GameObject gameObject; //保存弓箭对象
    public Vector3 point; //弓箭当前坐标
    public float speed; //弓箭移动速度
    public float moveDistance; //弓箭每步移动距离

}
