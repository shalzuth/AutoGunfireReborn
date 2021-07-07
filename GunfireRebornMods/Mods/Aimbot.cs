using UnityEngine;
using System.Runtime.InteropServices;
namespace GunfireRebornMods
{
    public class Aimbot : ModBase
    {
        [DllImport("user32.dll")] static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        bool Toggled = false;
        // thx https://github.com/pentium1131/GunfireReborn-aimbot
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.X)) Toggled = !Toggled;
            if (Toggled) return;
            var monsters = NewPlayerManager.GetMonsters();
            if (monsters != null)
            {
                Transform closestMonster = null;
                var closestMonsterDist = 99999f;
                foreach (var monster in monsters)
                {
                    var monsterTransform = monster.BodyPartCom.GetWeakTrans(true);
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
                {/*
                    var myPosition = HeroCameraName.HeroCameraManager.HeroObj.gameTrans.position;
                    //System.Console.WriteLine(myPosition.x + " : " + myPosition.y + " : " + myPosition.z);
                    //myPosition.y += 1;
                    var fw = closestMonster.position - myPosition;
                    //fw.y += 0.12f;
                    var rot = Quaternion.LookRotation(fw);
                   // HeroCameraName.HeroCameraManager.HeroObj.gameTrans.rotation = rot;
                    var camFw = closestMonster.position - CameraManager.MainCamera.position;
                    //fw.y += 0.12f;
                    var newCamRot = Quaternion.LookRotation(camFw);
                    //System.Console.WriteLine(CameraManager.MainCamera.position.x + " : " + CameraManager.MainCamera.position.y + " : " + CameraManager.MainCamera.position.z);
                    //System.Console.WriteLine(newCamRot.x + " : " + newCamRot.y + " : " + newCamRot.z);
                    //CameraManager.MainCamera.rotation = newCamRot;
                    */
                    var offset = closestMonster.position;
                    offset += new Vector3(0, 0.2f);
                    var shit = CameraManager.MainCameraCom.WorldToScreenPoint(offset);
                    //System.Console.WriteLine(shit.x + " : " + shit.y);
                    var AimTarget = new Vector2(shit.x, Screen.height - shit.y);
                    if (AimTarget != Vector2.zero)
                    {
                        double DistX = AimTarget.x - Screen.width / 2.0f;
                        double DistY = AimTarget.y - Screen.height / 2.0f;

                        System.Console.WriteLine(DistX + " : " + DistY);
                        //aimsmooth
                        DistX /= 2.5f;
                        DistY /= 2.5f;

                        mouse_event(0x0001, (int)DistX, (int)DistY, 0, 0);
                    }
                }
            }
        }
    }
}
