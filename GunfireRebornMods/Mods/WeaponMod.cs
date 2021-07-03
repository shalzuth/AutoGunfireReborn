using Il2CppSystem;
using UnityEngine;
namespace GunfireRebornMods
{
    public class WeaponMod : ModBase
    {
        Vector2 ScrollPos = Vector2.zero;
        public override void Update()
        {
            var ws = HeroMoveManager.HeroObj?.BulletPreFormCom?.weapondict;
            foreach (var w in ws)
            {
                if (w.value.WeaponAttr.Radius != 500f)  w.value.WeaponAttr.Radius = 500f;
                //if (w.value.WeaponAttr.Accuracy != 10000) w.value.WeaponAttr.Accuracy = 10000;
                if (w.value.WeaponAttr.AttDis != 500f) w.value.WeaponAttr.AttDis = 500f;
                // if (w.value.WeaponAttr.Pierce != 99) w.value.WeaponAttr.Pierce = 99; 
                if (w.value.WeaponAttr.BulletSpeed >= 50f && w.value.WeaponAttr.BulletSpeed != 55f && w.value.WeaponAttr.BulletSpeed != 500f || w.value.WeaponAttr.BulletSpeed == 30f) w.value.WeaponAttr.BulletSpeed = 500f;
                //if (w.value.WeaponAttr.Stability != 10000) w.value.WeaponAttr.Stability = 10000;
                if (w.value.WeaponAttr.Radius > 0f && w.value.WeaponAttr.Radius < 9f) w.value.WeaponAttr.Radius = 9f;
            }
        }
        public override void OnGUI()
        {
            return;
            var area = new Rect(Screen.width - 175, 25, 150, Screen.height - 50);
            GUI.Box(area, "weapon mods");
            GUILayout.BeginArea(area);
            GUILayout.Space(12);
            ScrollPos = GUILayout.BeginScrollView(ScrollPos, new GUILayoutOption[0]);
            var ws = HeroMoveManager.HeroObj?.BulletPreFormCom?.weapondict;
            foreach (var w in ws)
            {
                //GUILayout.Label("Weapon : " + w.Value.TypeName, new GUILayoutOption[0]);
                var dict = w.Value.WeaponAttr.m_PropDict;
                foreach (var a in dict)
                {
                    continue;
                    if (a.Value.PropValueType == PropType.TYPE_INTEGER)
                        GUILayout.Label(a.Key + " : " + a.Value.GetPropValue<System.Int32>(), new GUILayoutOption[0]);
                    else if (a.Value.PropValueType == PropType.TYPE_FLOAT)
                        GUILayout.Label(a.Key + " : " + a.Value.GetPropValue<System.Single>(), new GUILayoutOption[0]);
                    else if (a.Value.PropValueType == PropType.TYPE_LIST_STR)
                        GUILayout.Label(a.Key + " : " + a.Value.GetPropValue<System.String>(), new GUILayoutOption[0]);

                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
