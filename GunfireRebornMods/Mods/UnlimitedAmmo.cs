namespace GunfireRebornMods
{
    public class Ammo : ModBase
    {
        public override void Update()
        {
            var ws = HeroMoveManager.HeroObj?.BulletPreFormCom?.weapondict;
            foreach (var w in ws) w.value?.ReloadBulletImmediately();
            foreach (var w in ws) w.value?.ModifyBulletInMagzine(1, 100);
            // or Hook ConsumeBulletFromMag
        }
    }
}
