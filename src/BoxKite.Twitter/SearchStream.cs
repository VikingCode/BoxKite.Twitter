// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BoxKite.Twitter.Extensions;
using BoxKite.Twitter.Models;
using BoxKite.Twitter.Models.Stream;
using BoxKite.Twitter.Modules.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BoxKite.Twitter
{
    public class SearchStream : ISearchStream
    {
        readonly Subject<Tweet> foundtweets = new Subject<Tweet>();
        public IObservable<Tweet> FoundTweets { get { return foundtweets; } }

        private Subject<StreamSearchRequest> searchrequests = new Subject<StreamSearchRequest>();
        public Subject<StreamSearchRequest> SearchRequests
        {
            get { return searchrequests; }
        }

        public bool IsActive { get; private set; }

        public CancellationTokenSource CancelSearchStream { get; set; }
        public TwitterParametersCollection SearchParameters { get; set; }
        readonly Func<Task<HttpResponseMessage>> _createOpenConnection;
        public Func<Task<HttpResponseMessage>> CreateOpenConnection { get; set; }
        public IUserSession parentSession { get; set; }

        public SearchStream(IUserSession session)
        {
            parentSession = session;
            CancelSearchStream = new CancellationTokenSource();
            SearchRequests.Subscribe(ChangeSearchRequest);
            IsActive = true;
        }


        public void Start()
        {
            CancelSearchStream = new CancellationTokenSource();
            Task.Factory.StartNew(ProcessMessages, CancelSearchStream.Token);  
        }

        public void Stop()
        {
            IsActive = false;
        }

        private void ChangeSearchRequest(StreamSearchRequest sr)
        {
            CancelSearchStream.Cancel();
            SearchParameters = ChangeSearchParameters(sr);
            Start();
        }


        //https://dev.twitter.com/docs/streaming-apis/parameters
        public TwitterParametersCollection ChangeSearchParameters(StreamSearchRequest searchRequest)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (searchRequest.tracks.HasAny())
                parameters.CreateCommaDelimitedList("track", searchRequest.tracks);
            if (searchRequest.follows.HasAny())
                parameters.CreateCommaDelimitedList("follow", searchRequest.follows);
            if (searchRequest.locations.HasAny())
                parameters.CreateCommaDelimitedList("locations", searchRequest.locations);

            parameters.Add("filter_level", "none"); //note this can be none,low,medium
            // you can also add parameters.Add("language","en");

            return parameters;
        }

         public TwitterParametersCollection ChangeSearchParameters(string track = null, string follow = null, string locations = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", new List<string> { track });
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", new List<string> { follow });
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", new List<string> { locations });

            parameters.Add("filter_level", "none"); //note this can be none,low,medium
            // you can also add parameters.Add("language","en");

            return parameters;
        }

        public TwitterParametersCollection ChangeSearchParameters(IEnumerable<string> track = null, IEnumerable<string> follow = null, IEnumerable<string> locations = null)
        {
            var parameters = new TwitterParametersCollection();
            parameters.Create(stall_warnings: false, delimited: false);

            if (track != null)
                parameters.CreateCommaDelimitedList("track", track);
            if (follow != null)
                parameters.CreateCommaDelimitedList("follow", follow);
            if (locations != null)
                parameters.CreateCommaDelimitedList("locations", locations);

            parameters.Add("filter_level", "none"); //note this can be none,low,medium
            // you can also add parameters.Add("language","en");

            return parameters;
        }


        private async void ProcessMessages()
        {
            var responseStream = await GetStream();
            while (!CancelSearchStream.IsCancellationRequested)
            {
                string line;
                try
                {
                    line = responseStream.ReadLine();
                    if (string.IsNullOrWhiteSpace(line.Trim())) continue;

                    if (line == "ENDBOXKITESEARCHSTREAMTEST")
                    {
                        Stop();
                        responseStream.Dispose();
                        break;
                    }

                    var obj = JsonConvert.DeserializeObject<JObject>(line);
                    if (obj["in_reply_to_user_id"] != null)
                    {
                        foundtweets.OnNext(MapFromStreamTo<Tweet>(obj.ToString()));
                        continue;
                    }
                }
                catch (Exception)
                {
                    responseStream.Dispose();
                    break;
                }
            }
        }

        private async Task<StreamReader> GetStream()
        {
            var response = await CreateOpenConnection();
            var stream = await response.Content.ReadAsStreamAsync();
            var responseStream = new StreamReader(stream);
            return responseStream;
        }

        private static T MapFromStreamTo<T>(string t)
        {
            return JsonConvert.DeserializeObject<T>(t);
        }
    }
}
