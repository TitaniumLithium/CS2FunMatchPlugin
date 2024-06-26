# **CS2FunMatchPlugin CS2类表演赛娱乐插件**
a showmatch like plugin for fun

## Features

### random fun mode every round 每回合随机一种娱乐模式

### enable or disable modes in config 可配置各个模式是否启用 配置模式参数

### custom console-only(.cfg) modes loading 可自定义仅控制台命令的模式

## Avaliable Fun/Mode 当前可使用的娱乐模式

### Bullet Teleport 瞬移子弹
### HealTeammates 攻击治疗队友
get hurt continually, your teammates can heal you.
持续受到伤害 只有队友能帮你活下去
### Health Raid 攻击吸血
### 1000 Hp 1000血量
### Infinite Grenades 无限手雷
### Jump Or Die 地板烫脚
if you don't jump, you will get hurt.
不跳就会收到伤害
### NoClip On 启用飞行穿墙
will noclip on for a while.
每隔一段时间切换一次状态 包点无法起飞
### Shoot Exchange 换位子弹
### To The Moon 月球模式
low gravity, counterforce enabled (like that in space).
低重力 射击会有一定反作用力
### WNoStop 按住W不松手
if you don't forward, you will get hurt.
不按住w则会收到伤害
### Change Weapon ONShoot 射击换枪
### Drop Weapon ONShoot 射击丢枪
### FootBall Mode 足球模式
Ts aim to take soccer ball to CTspawn, while CTs will stop them. T需要将足球踢进CT出生点

## How to Install 如何安装

### 1.Install metamod & CounterstrikeSharp 安装metamod以及CounterstrikeSharp 
see [https://docs.cssharp.dev/docs/guides/getting-started.html](https://docs.cssharp.dev/docs/guides/getting-started.html)
### 2.Put dll to plugins folder 将插件放于指定目录
...\Counter-Strike Global Offensive\game\csgo\addons\counterstrikesharp\plugins\FunMatchPlugin\FunMatchPlugin.dll

## Plugin Console Commands 插件控制台相关指令
`fun_lists` Show all avaliable Fun/Mode 显示所有当前支持的娱乐模式

`fun_displayhelp` will display help of every mode on round start 每回合开始播报模式帮助信息 @css/root required

`!fun_displayhelp` will NOT display help of every mode 停止播报模式帮助信息 @css/root required

`fun_random` will load random mode per round automaticly 启用每回合随机模式 @css/root required

`!fun_random` will not load any random mode per round automaticly 停用每回合随机模式 @css/root required

`fun_load [num]` load certain mode by num (num can be found in command "fun_lists") won't affect random load 手动加载模式 对应的数字可以在"funlists"指令查到 与随机模式独立 @css/root required

`!fun_load` Unload mode you manually load (num can be found in command "fun_lists") 卸载手动加载的模式 @css/root required

## Config 插件配置

if you would like to configurate something, pls put config here 修改模式默认配置 请将配置文件放在此文件夹
...\Counter-Strike Global Offensive\game\csgo\addons\counterstrikesharp\configs\plugins\FunMatchPlugin

`FunMatchPlugin.json` will configure enable or disable modes and how modes work. 该文件会配置是否启用特定模式，特定模式的参数

`CustomModes.json` will add new custom console-only mode to this plugin. 该文件会配置仅控制台自定义新模式并加入游戏中
you will need .cfg files to add custom console-only mode. 该模式基于.cfg文件运行，请将你需要的指令放在指定的.cfg文件中

see [https://github.com/TitaniumLithium/CS2FunMatchPlugin/tree/main/config-example](config-example)
