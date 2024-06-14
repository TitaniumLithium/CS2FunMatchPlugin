# **CS2FunMatchPlugin CS2类表演赛娱乐插件**
a showmatch like plugin for fun

## Features

### random fun mode every round 每回合随机一种娱乐模式

## Avaliable Fun/Mode 当前可使用的娱乐模式

### Bullet Teleport 瞬移子弹
### HealTeammates 攻击治疗队友
持续受到伤害 只有队友能帮你活下去
### Health Raid 攻击吸血
### 1000 Hp 1000血量
### Infinite Grenades 无限手雷
### Jump Or Die 地板烫脚
不跳就会收到伤害
### NoClip On 启用飞行穿墙
### Shoot Exchange 换位子弹
### To The Moon 月球模式
低重力 且有一定后坐力
### WNoStop 按住W不松手
if you don't forward, you will get hurt.
不向前则会收到伤害
### Change Weapon ONShoot 射击换枪
### Drop Weapon ONShoot 射击丢枪

## How to Install 如何安装

### 1.Install metamod & CounterstrikeSharp 安装metamod以及CounterstrikeSharp 
see [https://docs.cssharp.dev/docs/guides/getting-started.html](https://docs.cssharp.dev/docs/guides/getting-started.html)
### 2.Put dll to plugins folder 将插件放于指定目录
...\Counter-Strike Global Offensive\game\csgo\addons\counterstrikesharp\plugins\FunMatchPlugin\FunMatchPlugin.dll

## Plugin Console Commands 插件控制台相关指令
`funlists` Show all avaliable Fun/Mode 显示所有当前支持的娱乐模式

`fun_displayhelp` will display help of every mode on round start 每回合开始播报模式帮助信息

`!fun_displayhelp` will NOT display help of every mode 停止播报模式帮助信息

`fun_random` will load random mode per round automaticly 启用每回合随机模式

`!fun_random` will not load any random mode per round automaticly 停用每回合随机模式

`fun_load [num]` load certain mode by num (num can be found in command "funlists") won't affect random load 手动加载模式 对应的数字可以在"funlists"指令查到 与随机模式独立

`!fun_load` Unload mode you manually load (num can be found in command "funlists") 卸载手动加载的模式
