using System.Collections.Generic;
using UnityEngine.XR;

namespace Saritasa.Controllers
{
    public static class DetectVR
    {
        /// <summary>
        /// Try to get current headset name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool TryGetHeadsetName(out string name)
        {
            List<InputDevice> inputDevices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, inputDevices);

            if (inputDevices.Count > 0)
            {
                name = inputDevices[0].name;
                return true;
            }
            else
            {
                name = "";
                return false;
            }
        }
    }
}