using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data / Combo")]
public class Combo : ScriptableObject
{
    public enum ComboKey { NONE, TRICKONE, TRICKTWO, GRINDTRICK }

    public new string name;
    public int score;

    public ComboKey[] comboRequirement = { ComboKey.NONE };
}
