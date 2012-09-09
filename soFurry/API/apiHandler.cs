using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace soFurry.API
{
    public enum sortBy
    {
        date = 0,
        title = 1,
        popularity = 2
    }

    public enum contentType
    {
        all = -1,
        stories = 0,
        art = 1,
        music = 2,
        journals = 3
    }

    public enum viewSource
    {
        all = 0,
        favourites = 1,
        watchlist = 2,
        folder = 3,
        groups = 4,
        search = 5,
        commission = 6,
        user = 7,
        featured = 8,
        highlights = 9,
        groupWatchlist = 10,
        combinedWatchlist = 11
    }

    public enum apiCall
    {
        user,
        submission,
        shouts
    }

    public class response : EventArgs
    {
        private String ApiResponse;
        public String Response
        {
            set { ApiResponse = value; }
            get { return this.ApiResponse; }
        }
    }

    public class ApiHandler
    {
        private String username = "";
        private String password = "";

        private String authenticationPadding = "@6F393fk6FzVz9aM63CfpsWE0J1Z7flEl9662X";
        private long currentAuthenticationSequence = 0;
        private String salt = String.Empty;

        public event FinishHandler finished;
        public delegate void FinishHandler(ApiHandler api, response e);

        public ApiHandler(String user, String pass) 
        {
            username = user;
            password = pass;
        }

        public ApiHandler() { }

        public async void fetchAPIResponse(String url)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync(url);

            String responseBodyAsText = await response.Content.ReadAsStringAsync();

            response apiResp = new response();
            apiResp.Response = responseBodyAsText;

            finished(this, apiResp);
        }

        public async void fetchAPIResponseAuthed(String url)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(username), "otpuser");
            form.Add(new StringContent(generateCombinedConnString()), "otphash");
            form.Add(new StringContent(currentAuthenticationSequence.ToString()), "otpsequence");

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsync(url, form);

            String responseBodyAsText = await response.Content.ReadAsStringAsync();

            response apiResp = new response();
            apiResp.Response = responseBodyAsText;

            JObject obj = JObject.Parse(responseBodyAsText);

            authenticationPadding = (string)obj["newPadding"];
            salt = (string)obj["salt"];
            currentAuthenticationSequence = (long)obj["newSequence"];

            finished(this, apiResp);
        }

        public String submissionDownloadLink(sortBy sort, contentType cType, viewSource vSource)
        {
            return String.Format("http://chat.sofurry.com/ajaxfetch.php?f=browse&sort={0}&contentType={1}&viewSource={2}&page=0", (int)sort, (int)cType, (int)vSource);
        }

        public String submissionDownloadLink(sortBy sort, contentType cType, viewSource vSource, int page)
        {
            return String.Format("http://chat.sofurry.com/ajaxfetch.php?f=browse&sort={0}&contentType={1}&viewSource={2}&page={3}", (int)sort, (int)cType, (int)vSource, page);
        }

        public String submissionDownloadLink(sortBy sort, contentType cType, viewSource vSource, int page, String additionalKey, String additionalValue)
        {
            return String.Format("http://chat.sofurry.com/ajaxfetch.php?f=browse&sort={0}&contentType={1}&viewSource={2}&page={3}&{4}={5}", (int)sort, (int)cType, (int)vSource, page, additionalKey, additionalValue);
        }

        public String apiURL(apiCall type, int id)
        {
            String baseUrl = String.Empty;
            switch (type)
            {
                case apiCall.shouts:
                    baseUrl = "Shouts";
                    break;
                case apiCall.submission:
                    baseUrl = "SubmissionDetails";
                    break;
                case apiCall.user:
                    baseUrl = "UserProfile";
                    break;
            }
            return "http://api2.sofurry.com/std/get" + baseUrl + "?id=" + id;
        }

        public String getUsersOwnProfile()
        {
            return "http://api2.sofurry.com/std/getUserProfile";
        }

        private String generateCombinedConnString()
        {
            return stringToMD5(stringToMD5(password + salt) + authenticationPadding + currentAuthenticationSequence);
        }

        private String stringToMD5(String preHash)
        {
            System.Text.UTF8Encoding decoder = new System.Text.UTF8Encoding();
            byte[] target = decoder.GetBytes(preHash);
            return MD5Core.GetHashString(target);
        }
    }
}
