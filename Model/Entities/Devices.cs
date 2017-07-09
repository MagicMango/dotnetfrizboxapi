using FritzBoxAPI.Model.Enum;
using System;

namespace FritzBoxAPI.Model.Entities
{
    public class Device
    {
        private DeviceStatus _Status;
        public DeviceStatus Status { set; get; }
        public void SetDeviceSatus(String status)
        {
            switch (status)
            {
                case "led_green":
                case "globe_online":
                    _Status = DeviceStatus.Online;
                    break;
                default:
                    _Status = DeviceStatus.Offline;
                    break;
            }
        }
        public string Type { set; get; }
        public string Name { set; get; }
        public string Url { set; get; }
    }
}
