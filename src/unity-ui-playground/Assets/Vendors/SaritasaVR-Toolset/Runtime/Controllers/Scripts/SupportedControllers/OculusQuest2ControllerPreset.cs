using System.Collections.Generic;

namespace Saritasa.Controllers
{
    public class OculusQuest2ControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "Oculus Quest 2",
            "oculus quest2",
            "Meta Quest 2",
            "meta quest2",
            "Quest 2",
            "Oculus Touch Controller OpenXR"
        };

        protected override string LeftControllerResourcePath => "OculusTouch2_Left";
        protected override string RightControllerResourcePath => "OculusTouch2_Right";
    }
}