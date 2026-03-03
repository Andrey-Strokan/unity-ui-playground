using System.Collections.Generic;

namespace Saritasa.Controllers
{
    // TODO: Check if we still need Pico controllers.
    public class PicoG2ControllerPreset : ControllerPreset
    {
        public override List<string> HeadSetNameVariants { get; set; } = new List<string>()
        {
            "Pico G 2",
            "Pico G2",
            "G 2",
            "G2",
        };

        // TODO: Add controllers for G2.
        protected override string LeftControllerResourcePath { get; }
        protected override string RightControllerResourcePath { get; }
    }
}