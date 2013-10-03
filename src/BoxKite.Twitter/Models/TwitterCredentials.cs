﻿// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System.Runtime.Serialization;

namespace BoxKite.Twitter.Models
{
    [DataContract]
    public class TwitterCredentials
    {
        /// <summary>
        /// Have these Credentials been validated
        /// </summary>
        [DataMember]
        public bool Valid { get; set; }

        /// <summary>
        /// ConsumerKey (ClientKey) associated with these Credentials
        /// In Twitter, the Consumer is the API consuming/using the API requests
        /// </summary>
        [DataMember]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// ConsumerSecret (ClientSecret) associated with these Credentials
        /// In Twitter, the Consumer is the API consuming/using the API requests
        /// </summary>
        [DataMember]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string TokenSecret { get; set; }


        [DataMember]
        public string ScreenName { get; set; }
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// Unverified Credentials are not valid
        /// </summary>
        [IgnoreDataMember]
        public static TwitterCredentials _null = new TwitterCredentials { Valid = false };

        [IgnoreDataMember]
        public static TwitterCredentials Null
        {
            get { return _null; }
        }
    }
}
