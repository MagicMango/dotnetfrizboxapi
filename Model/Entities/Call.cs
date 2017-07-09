using FritzBoxAPI.Model.Enum;
using System;

namespace FritzBoxAPI.Model.Entities
{
    public class Call
    {
        private CallStatus _Status;
        public CallStatus Status
        {
            get { return _Status; }
        }
        public void SetCallSatus(String status)
        {
            switch (status)
            {
                case "call_rejected":
                    _Status = CallStatus.Rejected;
                    break;
                default:
                    _Status = CallStatus.Unknown;
                    break;
            }
        }

        private TimeSpan _Time;
        public TimeSpan Time { get { return _Time; } }
        public void SetTime(String time)
        {
            String[] split = time.Split(':');
            _Time = new TimeSpan(Int32.Parse(split[0]), Int32.Parse(split[1]), 0);
        }

        public String Number { set; get; }
        public String Name { set; get; }
        public String Display { set; get; }

    }
}
