﻿// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;

namespace BoxKite.Twitter.Models
{
    public class UserInListCursored : TwitterControlBase
    {
        public long previous_cursor { get; set; }
        public IEnumerable<TwitterList> lists { get; set; }
        public long next_cursor { get; set; }
    }

}
