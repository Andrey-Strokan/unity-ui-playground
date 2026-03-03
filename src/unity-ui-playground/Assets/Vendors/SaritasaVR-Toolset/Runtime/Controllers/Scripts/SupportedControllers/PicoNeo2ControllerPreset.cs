using System.Collections.Generic;

namespace Saritasa.Controllers
{
    // TODO: Check if we still need Pico controllers.
    public class PicoNeo2ControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "Pico Neo 2",
            "Pico Neo2",
            "Pico Pico Neo 2",
            "Pico Pico Neo2",
            "Neo 2",
            "Neo2"
        };

        protected override string LeftControllerResourcePath => "PicoNeo2_Left";
        protected override string RightControllerResourcePath => "PicoNeo2_Right";
    }
}