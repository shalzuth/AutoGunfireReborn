using DataHelper;
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
        public string FightTypeToString(NewPlayerObject obj)
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
        public override void OnEnable()
        {
            foreach (var monster in NewPlayerManager.MonsterLst)
            {
                System.Console.WriteLine("MonsterLst : " + monster.FightType);
            }
            foreach (var monster in NewPlayerManager.PlayerDict)
            {
                var val = monster.Value;
                System.Console.WriteLine("PlayerDict : " + monster.Value.FightType);
            }
            foreach (var monster in NewPlayerManager.NpcLst)
            {
                System.Console.WriteLine("NpcLst : " + monster.FightType);
            }
        }
        public override void Update()
        {
            var mon = NewPlayerManager.GetMonsters();
            foreach (var m in mon) if (m.BloodBarCom != null) m.BloodBarCom.ShowBloodBar();
            //foreach (var m in NewPlayerManager.MonsterLst) if (m.BloodBarCom != null) m.BloodBarCom.ShowBloodBar();
        }
        public override void OnGUI()
        {
            foreach (var p in NewPlayerManager.PlayerDict)
            {
                var val = p.Value;
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
    }
}
