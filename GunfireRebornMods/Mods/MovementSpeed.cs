namespace GunfireRebornMods
{
    public class MovementSpeed : ModBase
    {
        public override bool HasConfig { get; set; } = true;
        public override float SliderMin { get; set; } = 0;
        public override float SliderVal { get; set; } = 10f;
        public override float SliderMax { get; set; } = 30f;
        float orig = 0;
        public override void Update()
        {
            if (orig == 0) orig = HeroMoveManager.HMMJS.maxForwardSpeed;
            //HeroMoveManager.HMMJS.SetSpeed(12);
            HeroMoveManager.HMMJS.maxForwardSpeed = HeroMoveManager.HMMJS.maxBackwardsSpeed = HeroMoveManager.HMMJS.maxSidewaysSpeed = SliderVal;
        }
        public override void OnDisable()
        {
            HeroMoveManager.HMMJS.maxForwardSpeed = HeroMoveManager.HMMJS.maxBackwardsSpeed = HeroMoveManager.HMMJS.maxSidewaysSpeed = orig;
        }
    }
}
