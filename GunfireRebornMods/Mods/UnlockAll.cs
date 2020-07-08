namespace GunfireRebornMods
{
    public class UnlockAll : ModBase
    {
        public override void OnEnable()
        {
            foreach (var kvp in CollectionManager.m_CollectionMonster)
            {
                var l = new Il2CppSystem.Collections.Generic.List<int>();
                l.Add(kvp.Key);
                CollectionManager.UnLockMonsterAt(l);
            }
            foreach (var kvp in CollectionManager.m_CollectionRelic)
                CollectionManager.UnlockRelicItem(kvp.Key);
                //CollectionManager.relic(kvp.Key);
            foreach (var kvp in CollectionManager.m_CollectionWeapon)
                //CollectionManager.UnlocWeaponItem(kvp.key);
                CollectionManager.UnLockWeaponAt(kvp.key);
        }
    }
}
