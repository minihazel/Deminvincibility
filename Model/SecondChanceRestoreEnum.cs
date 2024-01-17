using System.ComponentModel;

namespace Deminvincibility.Model;

public enum SecondChanceRestoreEnum
{
    [Description("None")]
    None,
    [Description("1HP")]
    OneHealth,
    [Description("Half Limb Health")]
    Half,
    [Description("Full Limb Health")]
    Full
}