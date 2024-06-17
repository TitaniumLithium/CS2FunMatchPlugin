using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace FunMatchPlugin;
public class FunMatchPluginConfig : BasePluginConfig
{
    [JsonPropertyName("BulletTeleport")] public bool IsFunBulletTeleportOn { get; set; } = true;
    [JsonPropertyName("ChangeWeaponOnShoot")] public bool IsFunChangeWeaponOnShootOn { get; set; } = true;
    [JsonPropertyName("FunDropWeaponOnShoot")] public bool IsFunDropWeaponOnShootOn { get; set; } = true;
    [JsonPropertyName("FunHealTeammates")] public bool IsFunHealTeammatesOn { get; set; } = true;
    [JsonPropertyName("FunHealTeammatesBurnAfterSecond")] public float FunHealTeammatesBurnAfterSecond { get; set; } = 1.0f;
    [JsonPropertyName("FunHealTeammatesBurnDamage")] public int FunHealTeammatesBurnDamage { get; set; } = 5;
    [JsonPropertyName("FunHealTeammatesHealValue")] public int FunHealTeammatesHealValue { get; set; } = 10;
    [JsonPropertyName("FunHealthRaid")] public bool IsFunHealthRaidOn { get; set; } = true;
    [JsonPropertyName("FunHealthRaidinitHP")] public int FunHealthRaidinitHP { get; set; } = 100;
    [JsonPropertyName("FunHealthRaidmaxRaid")] public int FunHealthRaidmaxRaid { get; set; } = 100;
    [JsonPropertyName("FunHealthRaidScale")] public float FunHealthRaidScale { get; set; } = 0.5f;
    [JsonPropertyName("FunHighHP")] public bool IsFunHighHPOn { get; set; } = true;
    [JsonPropertyName("FunHighHPmaxHP")] public int FunHighHPmaxHP { get; set; } = 1000;
    [JsonPropertyName("FunHighHParmor")] public int FunHighHParmor { get; set; } = 200;
    [JsonPropertyName("FunInfiniteGrenade")] public bool IsFunInfiniteGrenadeOn { get; set; } = true;
    [JsonPropertyName("FunJumpOrDie")] public bool IsFunJumpOrDieOn { get; set; } = true;
    [JsonPropertyName("FunJumpOrDieBurnAfterSecond")] public float FunJumpOrDieBurnAfterSecond { get; set; } = 1.0f;
    [JsonPropertyName("FunJumpOrDieBurnDamage")] public int FunJumpOrDieBurnDamage { get; set; } = 5;
    [JsonPropertyName("FunNoClip")] public bool IsFunNoClipOn { get; set; } = true;
    [JsonPropertyName("FunNoClipinterval")] public float FunNoClipinterval { get; set; } = 2.0f;
    [JsonPropertyName("FunPlayerShootExChange")] public bool IsFunPlayerShootExChangeOn { get; set; } = true;
    [JsonPropertyName("FunToTheMoon")] public bool IsFunToTheMoonOn { get; set; } = true;
    [JsonPropertyName("FunToTheMoongravity")] public float FunToTheMoongravity { get; set; } = (float)(800 * 0.166);
    [JsonPropertyName("FunToTheMoonBulletGiveAbsV")] public float FunToTheMoonBulletGiveAbsV { get; set; } = 100;
    [JsonPropertyName("FunWNoStop")] public bool IsFunWNoStopOn { get; set; } = true;
    [JsonPropertyName("FunWNoStopBurnAfterSecond")] public float FunWNoStopBurnAfterSecond { get; set; } = 0.5f;
    [JsonPropertyName("FunWNoStopBurnDamage")] public int FunWNoStopBurnDamage { get; set; } = 2;
}