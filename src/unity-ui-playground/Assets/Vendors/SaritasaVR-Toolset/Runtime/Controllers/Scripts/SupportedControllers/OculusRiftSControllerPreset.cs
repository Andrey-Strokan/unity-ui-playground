using System.Collections.Generic;

namespace Saritasa.Controllers
{
    public class OculusRiftSControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "Oculus Rift S",
            "Quest Touch Pro Controller OpenXR",
            "Meta Quest Touch Pro Controller OpenXR"
        };

        protected override string LeftControllerResourcePath => "OculusTouch_Left";
        protected override string RightControllerResourcePath => "OculusTouch_Right";
    }
}