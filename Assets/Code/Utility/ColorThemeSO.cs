using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "ColorThemeSO", menuName = "SO/ColorTheme")]
public class ColorThemeSO : ScriptableObject
{
    [Header("Blocks")]
    public Material[] BlockMaterials;
    public Color BlocksFontColor;
    [Space(5)]
    public Material[] DefenderBlockMaterials;
    public Color DefenderBlockFontColor;
    [Space(5)]
    public Material MinusBlockMaterial;
    public Color MinusBlocksFontColor;

    [Header("BG")]
    public Color BackgroundColor;

    [Header("GUI")]
    public Color GuiColor_1;
    public Color GuiColor_2;
    public Color GuiColor_3;
}