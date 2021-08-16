using System.Collections;
using System.Collections.Generic;
using TableConfig;
using UnityEngine;
using UnityEngine.UI;

/*
 * 读取csv数据存入ArmyModel
 */
public class CsvRead : MonoBehaviour
{
    public ArmyModel armyData; //保存读取的csv数据
    
    void Awake()
    {
        armyData = Readcsv();
    }
    
    public ArmyModel Readcsv()
    {
        TableManager<ArmyModel> armyModel = new TableManager<ArmyModel>();
        List<ArmyModel> list = armyModel.GetAllModel();
        
        //将数据存入army对象
        ArmyModel army = new ArmyModel();
        army.id = list[0].id;
        army.note = list[0].note;
        army.Name = list[0].Name;
        army.MaxHp = list[0].MaxHp;
        army.Atk = list[0].Atk;
        army.Def = list[0].Def;
        army.ShootSpeed = list[0].ShootSpeed;
        
        return army;
    }
}
