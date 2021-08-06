using System.Collections;
using System.Collections.Generic;
using TableConfig;
using UnityEngine;
using UnityEngine.UI;

/*
 * 读取csv数据
 */
public class Readcsv : MonoBehaviour
{
    public GameObject armyHP;
    public static ArmyModel army;
    // Start is called before the first frame update
    void Start()
    {
        readcsv();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void readcsv()
    {
        TableManager<ArmyModel> armyModel = new TableManager<ArmyModel>();
        List<ArmyModel> list = armyModel.GetAllModel();
        //将数据存入army对象
        army = new ArmyModel();
        army.id = list[0].id;
        army.note = list[0].note;
        army.Name = list[0].Name;
        army.MaxHp = list[0].MaxHp;
        army.Atk = list[0].Atk;
        army.Def = list[0].Def;
        army.ShootSpeed = list[0].ShootSpeed;
        armyHP.GetComponent<Text>().text = army.MaxHp.ToString();
    }
}
