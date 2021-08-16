### 1.整体框架  

 数据部分：读取csv数据，修改弓箭手血量、攻击力、攻速数据，修改敌人血量数据。     
 游戏运行：弓箭手可根据按键切换动作，按R跑步，按I闲置，按A弓箭手进行攻击动画，实例化弓箭进行移动攻击，与敌人发生触发，敌人扣血，并销毁当前弓箭，当敌人血量小于等于0时死亡，销毁敌人和所有弓箭。    

### 2.界面结构     

 SampleScene：主场景，包括EventControl、Ground（地板）、Archer（弓箭手，包括闲置、跑步、攻击动画）、Enemy（敌人）、Canvas     

 EventControl：空物体，用来绑定脚本。  
 Canvas：弓箭手部分包括ArcherHPSlider（弓箭手血量条）、ArcherHPText（弓箭手血量数字）、ArcherHPName（弓箭手血量名字）；敌人部分包括EnemyHPSlider（敌人血量条） 、EnemyHPText（敌人血量数字）、EnemyHPName（敌人血量名字）

 预制体有2个：FrostArcher（弓箭手，直接拖出来生成弓箭手和敌人）、IceArrow（弓箭）
			    
### 3.代码结构

| 类名             | 功能                                               | 调用关系                                                     |
| ---------------- | -------------------------------------------------- | ------------------------------------------------------------ |
| ArmyModel        | 保存士兵数据                                       | 被CsvReader类调用                                            |
| ArrowModel       | 保存弓箭数据                                       | 被ArrowController类调用                                      |
| CsvReader        | 读取csv中的士兵数据                                | 调用ArmyModel类，被ArcherController类和EnemyController类调用 |
| ArcherController | 进行弓箭手的血量修改、动作切换和攻击操作           | 调用CsvReader类、ArrowController类和EnemyController类，被EnemyController调用 |
| ArrowController  | 进行生成弓箭，弓箭移动，弓箭销毁操作               | 调用ArrowModel和EnemyController类，被ArcherController类和EnemyController类调用 |
| EnemyController  | 进行修改敌人血量、销毁敌人和判断弓箭与敌人发生触发 | 调用CsvReader类、ArrowController类和ArcherController类，被ArrowController类和ArcherController类调用 |


### 4.流程图

![flowPath](https://github.com/89trillion-hehuan/second_test/blob/main/FlowChart.png)
