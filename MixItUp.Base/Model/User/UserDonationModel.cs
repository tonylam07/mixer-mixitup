﻿using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace MixItUp.Base.Model.User
{
    public enum UserDonationSourceEnum
    {
        GawkBox,
        Streamlabs,
        Tiltify,
        TipeeeStream,
    }

    [DataContract]
    public class UserDonationModel
    {
        [DataMember]
        public UserDonationSourceEnum Source { get; set; }

        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string ImageLink { get; set; }

        [DataMember]
        public double Amount { get; set; }

        [DataMember]
        public DateTimeOffset DateTime { get; set; }

        [JsonIgnore]
        public string AmountText { get { return string.Format("{0:C}", Math.Round(this.Amount, 2)); } }
    }
}
