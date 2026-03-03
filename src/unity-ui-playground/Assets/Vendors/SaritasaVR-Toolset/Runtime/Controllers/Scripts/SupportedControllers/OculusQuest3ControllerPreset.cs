using System.Collections.Generic;

namespace Saritasa.Controllers
{
    public class OculusQuest3ControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "meta quest 3",
            "quest 3",
            "Quest Touch Plus Controller OpenXR",
            "Meta Quest Touch Plus Controller OpenXR"
        };

        protected override string LeftControllerResourcePath => "OculusTouchPlus_Left";
        protected override string RightControllerResourcePath => "OculusTouchPlus_Right";
    }
}