using System.Collections;
using System.Collections.Generic;
using TableConfig;
using UnityEngine;

/*
 * 士兵数据类，保存从csv中读取的数据
 */
public class ArmyModel : BaseModel
{
    public int id; //士兵id
    public string Name; //士兵名字
    public string note; //士兵简介
    public int MaxHp; //士兵hp
    public int Atk; //士兵攻击力
    public int Def; //士兵防御力
    public int ShootSpeed; //士兵攻速
    
    public override object Key()
    {
        return id;
    }
}
