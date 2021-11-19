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
                    if (monster == null) continue;
                    var bodyPartCom = monster.BodyPartCom;
                    if (bodyPartCom == null) continue;
                    var monsterTransform = bodyPartCom.GetWeakTrans(true);
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
                    var offset = closestMonster.position;
                    offset += new Vector3(0, 0.2f);
                    var screenAim = CameraManager.MainCameraCom.WorldToScreenPoint(offset);
                    var aimTarget = new Vector2(screenAim.x, Screen.height - screenAim.y);
                    if (aimTarget != Vector2.zero)
                    {
                        var x = aimTarget.x - Screen.width / 2.0f;
                        var y = aimTarget.y - Screen.height / 2.0f;
                        x /= 2.5f;
                        y /= 2.5f;
                        mouse_event(0x0001, (int)x, (int)y, 0, 0);
                    }
                }
            }
        }
    }
}
