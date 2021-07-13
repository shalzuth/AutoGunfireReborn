namespace GunfireRebornMods
{
    public class JumpHeight : ModBase
    {
        public override bool HasConfig { get; set; } = true;
        public override float SliderMin { get; set; } = 0;
        public override float SliderVal { get; set; } = 8f;
        public override float SliderMax { get; set; } = 30f;
        float orig = 0;
        public override void Update()
        {
            if (orig == 0) orig = HeroMoveManager.HMMJS.jumping.baseHeight; //HeroMoveManager.HMMJS.jumping.StoredDefaultHeight;// HeroMoveManager.HMMJS.jumping.baseHeight;
            HeroMoveManager.HMMJS.jumping.baseHeight = (SliderVal * SliderVal) / (HeroMoveManager.HMMJS.movement.gravity * 2);

            //HeroMoveManager.HMMJS?.SetJumperHeight(SliderVal);
            //HeroMoveManager.HMMJS.jumping.StoredDefaultHeight = HeroMoveManager.HMMJS.jumping.baseHeight;
        }
        public override void OnDisable()
        {
            HeroMoveManager.HMMJS.jumping.baseHeight = orig;
        }
    }
}
