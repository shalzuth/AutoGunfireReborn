using DataHelper;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace GunfireRebornMods
{
    public class ExtraSensoryPerception : ModBase
    {
        public bool ShowObject(NewPlayerObject obj)
        {
            if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_EQUIP) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_RELIC) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_GSCASHSHOP) return true;
            else if (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427)) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT) return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX) return true;
            return false;

        }
        public String FightTypeToString(NewPlayerObject obj)
        {
            if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_EQUIP) return DataMgr.GetWeaponData(obj.Shape).Name + " +" + obj.DropOPCom.WeaponInfo.SIProp.Grade.ToString();
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_RELIC) return DataMgr.GetRelicData(obj.DropOPCom.RelicSid).Name;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH) return "Smith";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP) return "Kermit";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_GSCASHSHOP) return "GhostKermit";
            else if (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427)) return "Vault";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT) return "Chest";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX) return "Chest";
            return "unk";
        }
        public override void OnGUI()
        {
            /*var monsters = GameObject.FindObjectsOfType<Transform>();
            
            foreach(var monster in monsters)
            {
                if (monster.gameObject.name.Contains("_effect")) continue;
                if (monster.gameObject.name.Contains("67")) continue;
                if (!monster.gameObject.name.Contains("earth")) continue;
                var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(monster.transform.position);
                if (screenPos.z > 0) GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 50, 50), monster.gameObject.name);
            }*/
            try
            {
                var mon = NewPlayerManager.GetMonsters();
                foreach (var m in mon)
                {
                    if (m.BloodBarCom != null) m.BloodBarCom.ShowBloodBar();
                }
                foreach (var monster in NewPlayerManager.MonsterLst)
                {
                    continue;
                    if (monster.centerPointTrans == null) continue;
                    var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(monster.centerPointTrans.transform.position);
                    if (screenPos.z > 0)
                    {
                        var dist = Vector3.Distance(HeroMoveManager.HeroObj.centerPointTrans.position, monster.centerPointTrans.position).ToString("0.0");
                        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 800, 50), DataMgr.GetMonsterGalleryData(monster.Shape).Name + "(" + dist + "m)");
                    }
                }
                foreach (var monster in NewPlayerManager.PlayerDict)
                {
                    var val = monster.Value;
                    if (val.centerPointTrans == null) continue;
                    if (!ShowObject(val)) continue;
                    var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(val.centerPointTrans.transform.position);
                    if (screenPos.z > 0)
                    {
                        var dist = Vector3.Distance(HeroMoveManager.HeroObj.centerPointTrans.position, val.centerPointTrans.position).ToString("0.0");
                        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 800, 50), FightTypeToString(val) + "(" + dist + "m)");
                        //GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 800, 50), monster.Value.SID + " : " + monster.Value.Shape + " : " + monster.Value.FightType);
                    }
                }
            }
            catch { }
        }
    }
}
