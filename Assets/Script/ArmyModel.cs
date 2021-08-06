using System.Collections;
using System.Collections.Generic;
using TableConfig;
using UnityEngine;

/*
 * 弓箭手各属性
 */
public class ArmyModel : BaseModel
{
    public int id; //弓箭手id
    public string Name; //弓箭手名字
    public string note; //弓箭手简介
    public int MaxHp; //弓箭手hp
    public int Atk; //弓箭手攻击力
    public int Def; //弓箭手防御力
    public int ShootSpeed; //弓箭手攻速
    public override object Key()
    {
        return id;
    }
}
