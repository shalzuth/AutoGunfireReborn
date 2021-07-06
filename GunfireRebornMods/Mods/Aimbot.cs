using UnityEngine;
namespace GunfireRebornMods
{
    public class Aimbot : ModBase
    {
        // thx https://github.com/pentium1131/GunfireReborn-aimbot
        public override void Update()
        {
            if (!Input.GetKey(KeyCode.X)) return;
            var monsters = NewPlayerManager.GetMonsters();
            if (monsters != null)
            {
                Transform closestMonster = null;
                var closestMonsterDist = 99999f;
                foreach (var monster in monsters)
                {
                    var monsterTransform = monster.BodyPartCom.GetWeakTrans();
                    if (monsterTransform == null) continue;
                    var vec = CameraManager.MainCameraCom.WorldToViewportPoint(monsterTransform.position);
                    if (true)
                    {
                        if (vec.z <= 0) continue;
                        vec.y = 0;
                        vec.x = 0.5f - vec.x;
                        vec.x = Screen.width * vec.x;
                        vec.z = 0f;
                        if (vec.magnitude > 150f) continue;
                    }
                    vec = monsterTransform.position - CameraManager.MainCamera.position;
                    var ray = new Ray(CameraManager.MainCamera.position, vec);
                    var hits = Physics.RaycastAll(ray, vec.magnitude);
                    var visible = true;
                    foreach (var hit in hits)
                    {
                        if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 30 || hit.collider.gameObject.layer == 31) //&& hit.collider.name.Contains("_")
                        {
                            visible = false;
                            break;
                        }
                    }
                    if (visible)
                    {
                        if (vec.magnitude < closestMonsterDist)
                        {
                            closestMonsterDist = vec.magnitude;
                            closestMonster = monsterTransform;
                        }
                    }


                }
                if (closestMonster != null)
                {
                    var myPosition =  HeroCameraName.HeroCameraManager.HeroObj.gameTrans.position;
                    myPosition.y += 1;
                    var fw = closestMonster.position - myPosition;
                    //fw.y += 0.12f;
                    var rot = Quaternion.LookRotation(fw);
                    HeroCameraName.HeroCameraManager.HeroObj.gameTrans.rotation = rot;
                    var camFw = closestMonster.position - CameraManager.MainCamera.position;
                    //fw.y += 0.12f;
                    var newCamRot = Quaternion.LookRotation(camFw);
                    CameraManager.MainCamera.rotation = newCamRot;

                }
            }
        }
    }
}
