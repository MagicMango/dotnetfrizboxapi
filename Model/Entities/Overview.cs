using System.Collections.Generic;

namespace FritzBoxAPI.Model.Entities
{
    public class Overview
    {
        public List<Call> Calls { set; get; }
        public List<Device> Devices { set; get; }

        public Overview()
        {
            Calls = new List<Call>();
            Devices = new List<Device>();
        }
    }
}
