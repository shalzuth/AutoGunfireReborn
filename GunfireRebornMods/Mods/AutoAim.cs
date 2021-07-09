using UnityEngine;
namespace GunfireRebornMods
{
    public class AutoAim : ModBase
    {
        public override void OnEnable()
        {
            GameObject.FindObjectOfType<PCControllerAutoShoot_Logic>().speed = 100;            
        }
        public override void Update()
        {
            AutoAimat.AimAtTarget(100f);
            DetectionClass.aimAssist = true;
        }
        public override void OnDisable()
        {
            GameObject.FindObjectOfType<PCControllerAutoShoot_Logic>().speed = 0;
        }
    }
}
