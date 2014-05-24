﻿// (c) 2012-2014 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter;
using BoxKite.Twitter.Authentication;
using BoxKite.Twitter.Models;
using Reactive.EventAggregator;

namespace BoxKite.Twitter.Console
{
    public class ApplicationOnlyAuthFireTests
    {
        public async Task<bool> DoCombosTest(TwitterConnection twitterConnection, List<int> testSeq)
        {
            var successStatus = true;

            try
            {
                // 1
                if (testSeq.Contains(1))
                {
                    ConsoleOutput.PrintMessage("12.1 User Time Line//Application Auth Only", ConsoleColor.Gray);

                    var combo1 = await twitterConnection.ApplicationSession.GetUserTimeline("KatyPerry");

                    if (combo1.OK)
                    {
                        foreach (var trnd in combo1)
                        {
                            ConsoleOutput.PrintMessage(
                                String.Format("Tweet Test: {0}", trnd.Text));
                        }
                    }
                    else
                    {
                        successStatus = false;
                        throw new Exception("cannot user time line app only auth");
                    }
                } // end test 1

            }
            catch (Exception e)
            {
                ConsoleOutput.PrintError(e.ToString());
                return false;
            }
            return successStatus;
        }
    }
}