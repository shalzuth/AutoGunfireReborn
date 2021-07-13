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
    }
}
