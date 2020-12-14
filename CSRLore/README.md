# CSRLore
添加lore注释

#### 指令说明
/lore [说明文本]
    玩家：将手持装备添加上自定义装备注释说明。
    [例] /lore "今天天气不错"--鲁迅
/loreraw [说明JSON文本]
    玩家：将手持装备添加上自定义装备注释说明集合，以JSON格式的文本添加。需要texts集合关键字。(注：如果texts集合为空集[]时，效果为移除当前装备的lore信息)
    [例] /loreraw {"texts":["","感觉自己呵呵哒"," --鲁迅"]}
lore [reload]
    后台：重载配置文件
    [例] lore reload
    
#### 开发指导
    详见 LoreApi.cs 注释内详
