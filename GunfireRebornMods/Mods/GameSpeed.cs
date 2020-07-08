namespace GunfireRebornMods
{
    public class GameSpeed : ModBase
    {
        public override bool HasConfig { get; set; } =  true;
        public override void Update()
        {
            UnityEngine.Time.timeScale = SliderVal;
        }
        public override void OnDisable()
        {
            UnityEngine.Time.timeScale = 1.0f;
        }
    }
}
