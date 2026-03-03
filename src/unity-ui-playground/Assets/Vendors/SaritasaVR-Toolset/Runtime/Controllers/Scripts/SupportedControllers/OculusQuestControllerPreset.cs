using System.Collections.Generic;

namespace Saritasa.Controllers
{
    public class OculusQuestControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "Oculus Quest",
            "Meta Quest",
            "Quest"
        };

        protected override string LeftControllerResourcePath => "OculusTouch_Left";
        protected override string RightControllerResourcePath => "OculusTouch_Right";
    }
}