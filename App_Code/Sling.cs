using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

/// <summary>
/// Summary description for Sling
/// </summary>
public class Sling
{
    protected string sBaseUrl = @"https://api.sling.is/v1/";
    protected string sEmail;
    protected string sPassword;
    protected string sOrgId;
    protected string sUserId;
    protected string sLocation;
    protected string sPosition;
    protected string sUserEmail;
    protected bool fDebug = false;
    protected bool fDebugLog = false;
    protected StreamWriter LogFile;

    protected string Authorization;
    protected bool fUsersLoaded;
    protected bool fGroupsLoaded;
    protected Collection<User> cUsers = new Collection<User>();
    protected Collection<Group> cGroups = new Collection<Group>();
    protected Collection<CalendarEvent> cShifts = new Collection<CalendarEvent>();

    protected string sScheme;
    protected string sHost;
    protected string sPort;

    protected string sErrorMessage = string.Empty;

    protected const string IsoDateTime = @"yyyy-MM-ddTHH\:mm\:ss.fffzzz";
    protected const string UserTZName = "Mountain Standard Time";
    protected TimeZoneInfo UserTZ = TimeZoneInfo.FindSystemTimeZoneById(UserTZName);


    public string Email { get { return sEmail; } }
    public string Password { get { return sPassword; } }
    public string OrgId { get { return sOrgId; } }
    public string UserId {  get { return sUserId; } }
    public string Location {  get { return sLocation;  } }
    public string Position { get { return sPosition; } }
    public string UserEmail { get { return sUserEmail; } }
    public Collection<User> Users { get { return cUsers; } }
    public Collection<Group> Groups { get { return cGroups; } }
    public Collection<CalendarEvent> Shifts { get { return cShifts; } }
    public string Scheme { get { return sScheme; } }
    public string Host { get { return sHost; } }
    public string Port { get { return sPort; } }


    public string ErrorMessage { get { return sErrorMessage; } }


    // debugging
    //TextWriterTraceListener tl = new TextWriterTraceListener("C:\\Temp\\Debug.txt");
    //Debug.Listeners.Add(tl);
    //Debug.AutoFlush = true;

    //Debug.Listeners.Remove(tl);


    public Sling()
    {
        WebConfig wc = new WebConfig();
        sEmail = wc.Str("SlingEmail");
        sPassword = wc.Str("SlingPassword");
        sOrgId = string.Empty;
        sUserId = string.Empty;
        sLocation = wc.Str("SlingLocation");
        sPosition = wc.Str("SlingPosition");
        sUserEmail = SysParm.Get(Misc.cnSlingUserEmail);
        fDebug = wc.Bool("SlingDebug");
        fDebugLog = wc.Bool("SlingLog");

        sScheme = HttpContext.Current.Request.Url.Scheme;
        sHost = HttpContext.Current.Request.Url.Host;
        sPort = (HttpContext.Current.Request.Url.Port == 80 ||
                 HttpContext.Current.Request.Url.Port == 443 ?
                   string.Empty :
                   string.Format(":{0}", HttpContext.Current.Request.Url.Port));

        if (HttpContext.Current.Session["SlingAuth"] != null)
        {
            Authorization = HttpContext.Current.Session["SlingAuth"].ToString();
        }
        if (HttpContext.Current.Session["SlingOrg"] != null)
        {
            sOrgId = HttpContext.Current.Session["SlingOrg"].ToString();
            sBaseUrl = string.Format(@"https://api.sling.is/v1/{0}/", sOrgId);
            sBaseUrl = @"https://api.sling.is/v1/";
        }
        if (HttpContext.Current.Session["SlingUser"] != null)
        {
            sUserId = HttpContext.Current.Session["SlingUser"].ToString();
        }

        if (fDebug && fDebugLog)
        {
            LogFile = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SlingLog.txt"), true);
        }
    }
    
    protected string IsoUserTZ(DateTime dt)
    {
        try
        {
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            DateTimeOffset dto = new DateTimeOffset(dt, (TimeSpan)UserTZ.GetUtcOffset(dt));       // date and time in user's timezone offset based on the given date

            return TimeZoneInfo.ConvertTime(dto, UserTZ).ToString(IsoDateTime);         // change to ISO date time format
        }
        catch (ArgumentException Ex)
        {
            throw new Exception("Argument Exception: DateTime " + dt.ToString() + " UTC Offset " + UserTZ.GetUtcOffset(dt).ToString(), Ex);
        }
    }

    protected DateTime UserTime(DateTime dt)
    {
        return TimeZoneInfo.ConvertTime(dt, UserTZ);
    }

    public async Task<bool> Login(string Email, string Password)
    {
        bool Result = false;
        bool fSpecialLogin = (Email != this.Email);

        if (Authorization == null || fSpecialLogin)
        {
            string Json = string.Format(
                "{{" +
                    "\"email\":\"{0}\"," +
                    "\"password\":\"{1}\"" +
                "}}",
                Email,
                Password);
            StringContent Content = new StringContent(Json);
            Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage Response = await new HttpClient().PostAsync(sBaseUrl + "account/login", Content);

            await DebugHttpCall(Json, Response);

            if (Response.StatusCode == HttpStatusCode.OK)
            {
                string Resp = await Response.Content.ReadAsStringAsync();
                IEnumerable<string> Authorizations;
                if (Response.Headers.TryGetValues("Authorization", out Authorizations))
                {
                    IEnumerator<string> Auth = Authorizations.GetEnumerator();
                    Auth.MoveNext();
                    Authorization = Auth.Current;
                }

                JObject jData = JObject.Parse(Resp);
                sOrgId = (string)jData["org"]["id"];
                sUserId = (string)jData["user"]["id"];

                sBaseUrl = string.Format(@"https://api.sling.is/v1/{0}/", sOrgId);

                if (!fSpecialLogin)
                {
                    HttpContext.Current.Session["SlingAuth"] = Authorization;
                    HttpContext.Current.Session["SlingOrg"] = sOrgId;
                    HttpContext.Current.Session["SlingUser"] = sUserId;
                }
            }
        }

        if (Authorization != null && Authorization.Length > 0)
        {
            Result = true;
        }

        return Result;
    }

    public async Task<bool> Relogin()
    {
        Reset();
        return await Login(Email, Password);
    }

    public void Reset()
    {
        Authorization = null;
        sOrgId = null;
        sUserId = null;
        sBaseUrl = @"https://api.sling.is/v1/";
        Clear();
    }

    public void Clear()
    {
        HttpContext.Current.Session["SlingAuth"] = null;
        HttpContext.Current.Session["SlingOrg"] = null;
        HttpContext.Current.Session["SlingUser"] = null;
    }

    public async Task<int> CreateShift(DateTime tmBeg, DateTime tmEnd, string Location, string Position, string Summary)
    {
        int ID = 0;
        sErrorMessage = string.Empty;

        if (tmBeg > tmEnd)
        {
            tmEnd = tmEnd.AddDays(1);
        }

        int LocationId = await GetLocationId(Location);
        int PositionId = await GetPositionId(Position);

        string Json = string.Format(
            "{{" +
                "\"dtstart\": \"{0}\", " +
                "\"dtend\": \"{1}\", " +
                "\"location\": {{ \"id\": {2} }}, " +
                "\"position\": {{ \"id\": {3} }}, " +
                "\"summary\": \"{4}\"" +
            "}}",
            IsoUserTZ(tmBeg),
            IsoUserTZ(tmEnd),
            LocationId,
            PositionId,
            Summary.Replace("\"", "\\\""));
        StringContent Content = new StringContent(Json);
        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.PostAsync(sBaseUrl + "shifts?ignoreConflicts=true", Content);

        await DebugHttpCall(Json, Response);

        string Resp = null;
        switch (Response.StatusCode)
        {
            case HttpStatusCode.Created:        
                Resp = null;
                try
                {
                    Resp = await Response.Content.ReadAsStringAsync();
                    JArray aResp = JArray.Parse(Resp);
                    //JObject o = (JObject)a[0];
                    //Resp = Resp.Substring(1, Resp.Length - 3);  // remove leading [ and trailing ], make parseable
                    //JObject jData = JObject.Parse(Resp);
                    JObject jData = (JObject)aResp[0];
                    ID = (int)jData["id"];
                }
                catch (JsonReaderException Ex)
                {
                    WriteDebugLine("JsonReaderException: " + Ex.ToString());
                    WriteDebugLine(Resp);
                }
                catch (Exception Ex)
                {
                    WriteDebugLine("Exception: " + Ex.ToString());
                    WriteDebugLine(Resp);
                }
                break;

            case HttpStatusCode.Unauthorized:
                await Relogin();
                await CreateShift(tmBeg, tmEnd, Location, Position, Summary);
                break;

            default:
                sErrorMessage = string.Format("Sling Status Code: {0}<br>\n", Response.StatusCode);
                Resp = null;
                try
                {
                    Resp = await Response.Content.ReadAsStringAsync();
                    JObject jData = JObject.Parse(Resp);

                    if (jData["message"] != null)
                    {
                        sErrorMessage += string.Format("Message: {0}<br>\n", (string)jData["message"]);
                    }
                    if (jData["errors"] != null)
                    {
                        sErrorMessage += "Errors:<ul>\n";
                        foreach (string sError in jData["errors"])
                        {
                            sErrorMessage += string.Format("<li>{0}</li>\n", sError);
                        }
                        sErrorMessage += "</ul>\n";
                    }
                }
                catch (Exception Ex)
                {
                    WriteDebugLine("Exception: " + Ex.ToString());
                    WriteDebugLine(Resp);
                }
                break;
        }

        return ID;
    }

    public async Task<int> UpdateShift(int Id, DateTime tmBeg, DateTime tmEnd, string Location, string Position, string Summary)
    {
        sErrorMessage = string.Empty;
        int LocationId = await GetLocationId(Location);
        int PositionId = await GetPositionId(Position);

        string Json = string.Format(
            "{{" +
                "\"dtstart\": \"{0}\", " +
                "\"dtend\": \"{1}\", " +
                "\"location\": {{ \"id\": {2} }}, " +
                "\"position\": {{ \"id\": {3} }}, " +
                "\"summary\": \"{4}\"" +
            "}}",
            IsoUserTZ(tmBeg),
            IsoUserTZ(tmEnd),
            LocationId,
            PositionId,
            Summary);
        string a = Json;
        StringContent Content = new StringContent(Json);
        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.PutAsync(string.Format("{0}shifts/{1}?ignoreConflicts=true", sBaseUrl, Id), Content);

        await DebugHttpCall(Json, Response);

        switch (Response.StatusCode)
        {
            case HttpStatusCode.OK:
                break;

            case HttpStatusCode.Unauthorized:
                await Relogin();
                await UpdateShift(Id, tmBeg, tmEnd, Location, Position, Summary);
                break;

            default:
                sErrorMessage = string.Format("Sling Status Code: {0}<br>\n", Response.StatusCode);
                string Resp = null;
                try
                {
                    Resp = await Response.Content.ReadAsStringAsync();
                    JObject jData = JObject.Parse(Resp);

                    if (jData["message"] != null)
                    {
                        sErrorMessage += string.Format("Message: {0}<br>\n", (string)jData["message"]);
                    }
                    if (jData["errors"] != null)
                    {
                        sErrorMessage += "Errors:<ul>\n";
                        foreach (string sError in jData["errors"])
                        {
                            sErrorMessage += string.Format("<li>{0}</li>\n", sError);
                        }
                        sErrorMessage += "</ul>\n";
                    }
                }
                catch (Exception Ex)
                {
                    WriteDebugLine("Exception: " + Ex.ToString());
                    WriteDebugLine(Resp);
                }
                break;
        }

        return 0;
    }

    public async Task<int> DeleteShift(int Id)
    {
        StringContent Content = new StringContent("{}");
        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.DeleteAsync(string.Format("{0}shifts/{1}", sBaseUrl, Id));

        await DebugHttpCall(string.Empty, Response);

        return 0;
    }

    public async Task<int> PublishShift(int Id)
    {
        StringContent Content = new StringContent("{}");
        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.PostAsync(sBaseUrl + "shifts/sync", Content);

        await DebugHttpCall(string.Empty, Response);

        return 0;
    }

    public async Task<int> DebugHttpCall(string Json, HttpResponseMessage Response)
    {
        WriteDebugLine("<Request>");
        WriteDebugLine(Response.RequestMessage.ToString());
        WriteDebugLine(Json);
        WriteDebugLine("<Response>");
        WriteDebugLine(Response.ToString());
        WriteDebugLine(await Response.Content.ReadAsStringAsync());

        return 0;
    }

    protected void WriteDebugLine(string Line)
    {
        if (fDebug)
        {
            if (fDebugLog)
            {
                LogFile.WriteLine(Line);
                LogFile.Flush();
            }
            else
            {
                Debug.WriteLine(Line);
            }
        }
    }

    public async Task<int> GetLocationId(string Name)
    {
        return await GetGroupId("location", Name);
    }

    public async Task<int> GetPositionId(string Name)
    {
        return await GetGroupId("position", Name);
    }

    public async Task<int> GetGroupId(string Type, string Name)
    {
        //WriteDebugLine("GetGroupID " + Type + " " + Name);
        int ID = 0;

        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.GetAsync(string.Format("{0}groups?type={1}", sBaseUrl, Type));

        await DebugHttpCall(string.Empty, Response);

        if (Response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                string ResponseContents = await Response.Content.ReadAsStringAsync();
                //WriteDebugLine(ResponseContents);
                // wrap in valid json code that can be parsed
                string Resp = string.Format("{{\"Resp\":{0}}}", ResponseContents);
                //WriteDebugLine(Resp);
                JObject jData = JObject.Parse(Resp);
                JArray Groups = (JArray)jData["Resp"];
                for (int i = 0; i < Groups.Count; i++)
                {
                    if ((string)Groups[i]["name"] == Name)
                    {
                        ID = (int)Groups[i]["id"];
                        break;
                    }
                }
                //WriteDebugLine(ID);
            }
            catch (JsonReaderException Ex)
            {
                WriteDebugLine("JsonReaderException: " + Ex.ToString());
            }
            catch (Exception Ex)
            {
                WriteDebugLine("Exception: " + Ex.ToString());
            }
        }

        return ID;
    }

    public async Task<int> AddToPrivateConversation(string Data)
    {
        StringContent Content = new StringContent(Data);
        Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.PostAsync(sBaseUrl + "conversations", Content);

        await DebugHttpCall(Data, Response);

        if (Response.StatusCode == HttpStatusCode.OK)
        {
        }

        return 0;
    }

    public async Task<int> LoadUsers(bool fReload = false)
    {
        if (fReload)
        {
            cUsers = new Collection<User>();
            fUsersLoaded = false;
        }

        if (!fUsersLoaded)
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("Authorization", Authorization);
            HttpResponseMessage Response = await Client.GetAsync(sBaseUrl + "users/concise");

            await DebugHttpCall(string.Empty, Response);

            switch (Response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string UserData = await Response.Content.ReadAsStringAsync();
                    LoadUsers(UserData);
                    break;

                case HttpStatusCode.Unauthorized:
                    await Relogin();
                    await LoadUsers(fReload);
                    break;
            }
        }

        return 0;
    }

    protected void LoadUsers(string Data)
    {
        try
        {
            JObject jUserData = JObject.Parse(Data);

            if (jUserData["groups"] != null)
            {
                LoadGroups(jUserData["groups"]);
            }

            JToken jUsers = jUserData["users"];
            foreach (JToken jUser in jUsers)
            {
                if ((bool)jUser["active"])
                {
                    User User = new User((int)jUser["id"], (string)jUser["name"], (string)jUser["lastname"], (string)jUser["email"]);
                    cUsers.Add(User);
                    foreach (int ID in jUser["groupIds"])
                    {
                        Group Group = FindGroup(ID);
                        if (Group != null)
                        {
                            Group.Users.Add(User);
                        }
                    }
                }
            }

            fUsersLoaded = true;
        }
        catch (Exception)
        {
        }
    }

    public void SortUsers(Collection<User> Users)
    {
        for (int i = 0; i < Users.Count - 1; i++)
        {
            for (int j = i + 1; j < Users.Count; j++)
            {
                User UserA = Users[i];
                User UserB = Users[j];

                int Comp = string.Compare(UserA.FirstName.ToLower(), UserB.FirstName.ToLower());
                if (Comp == 0)
                {
                    Comp = string.Compare(UserA.LastName.ToLower(), UserB.LastName.ToLower());
                }
                if (Comp > 0)
                {
                    Users[i] = UserB;
                    Users[j] = UserA;
                }
            }
        }
    }

    public async Task<User> FindUser(int ID)
    {
        User Found = null;

        await LoadUsers();
        foreach (User User in Users)
        {
            if (User.ID == ID)
            {
                Found = User;
                break;
            }
        }

        return Found;
    }


    public async Task<User> FindUserBy(string Field, string Value)
    {
        User Found = null;

        await LoadUsers();
        foreach (User User in Users)
        {
            if (Field == "ID" && User.ID.ToString() == Value ||
                Field == "FirstName" && User.FirstName == Value ||
                Field == "LastName" && User.LastName == Value ||
                Field == "Email" && User.Email == Value)
            {
                Found = User;
                break;
            }
        }

        return Found;
    }

    protected void LoadGroups(JToken jGroupData)
    {
        if (jGroupData != null)
        {
			// using /users/concise
			foreach (JToken jGroup in jGroupData)
            {
                JToken jData = jGroup.First();
                Group Group = new Group((int)jData["id"], (string)jData["name"], (string)jData["type"]);
                Groups.Add(Group);
                // users will be connected a little later
            }
        }
        /*
        if (data)
        {
			// using /users/concise
			for (let groupId in data)
            {
                let groupData = data[groupId];
                let group = new Group(groupData.id, groupData.name, groupData.type);
                this._groups.push(group);
                // users will be connected a little later
            }
            if (callBack)
            {
                callBack();
            }
        }
        else
        {
            // using /users
            this._sling.getGroups((data) =>
            {
                for (let groupData of data)
                {
                    let group = new Group(groupData.id, groupData.name, groupData.type);
                    this._groups.push(group);
                    this._connectUsers(group);
                }
                if (callBack)
                {
                    callBack();
                }
            });
        }
        */
    }

    public Group FindGroup(int ID)
    {
        Group Found = null;

        foreach (Group Group in Groups)
        {
            if (Group.ID == ID)
            {
                Found = Group;
                break;
            }
        }

        return Found;
    }

    public Group FindGroupByName(string Name)
    {
        Group Found = null;

        foreach (Group Group in Groups)
        {
            if (Group.Name == Name)
            {
                Found = Group;
                break;
            }
        }

        return Found;
    }

    public void ClearCalendar()
    {
        foreach (User User in Users)
        {
            User.ClearCalendarEvents();
        }
    }

    public async Task<bool> LoadCalendar(DateTime dtBeg, DateTime dtEnd)
    {
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);

        HttpResponseMessage Response = await Client.GetAsync(string.Format(
            "{0}calendar/{1}/users/{2}?dates={3}/{4}", 
            sBaseUrl, 
            OrgId, 
            UserId, 
            IsoUserTZ(dtBeg), 
            IsoUserTZ(dtEnd)));

        //TextWriterTraceListener tl = new TextWriterTraceListener("C:\\Temp\\Debug.txt");
        //Debug.Listeners.Add(tl);
        //Debug.AutoFlush = true;

        await DebugHttpCall(string.Empty, Response);

        //Debug.Listeners.Remove(tl);

        if (Response.StatusCode == HttpStatusCode.OK)
        {
            string Resp = await Response.Content.ReadAsStringAsync();
            Resp = string.Format("{{\"Resp\":{0}}}", Resp);     // make the data readable for JObject.Parse()
            //JObject jData = JObject.Parse(Resp);
            JObject jData = (JObject)JsonConvert.DeserializeObject(Resp, new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTimeOffset });
            await ProcessCalendar(jData);
            //WriteDebugLine("LoadCalendar # shifts " + Shifts.Count.ToString());
        }
        else
        {
            throw new Exception(string.Format("Sling API error. URL: {0}, Response Status Code: {1} - {2}", Response.RequestMessage.ToString(), (int)Response.StatusCode, Response.ReasonPhrase));
        }

        return true;
    }

    //public async Task<bool> LoadCalendar(DateTime dtBeg, DateTime dtEnd)
    //{
    //    HttpClient Client = new HttpClient();
    //    Client.DefaultRequestHeaders.Add("Authorization", Authorization);
    //    HttpResponseMessage Response = await Client.GetAsync(string.Format("{0}calendar/available?dates={1}/{2}", sBaseUrl, dtBeg.ToString(IsoDateTime), dtEnd.ToString(IsoDateTime)));

    //    await DebugHttpCall(string.Empty, Response);

    //    if (Response.StatusCode == HttpStatusCode.OK)
    //    {
    //        string Resp = await Response.Content.ReadAsStringAsync();
    //        Resp = string.Format("{{\"Resp\":{0}}}", Resp);     // make the data readable for JObject.Parse()
    //        JObject jData = JObject.Parse(Resp);
    //        ProcessCalendar(jData);
    //    }
    //    else
    //    {
    //        throw new Exception(string.Format("Sling API error. URL: {0}, Response Status Code: {1} - {2}", Response.RequestMessage.ToString(), (int)Response.StatusCode, Response.ReasonPhrase));
    //    }

    //    return true;
    //}

    protected async Task ProcessCalendar(JObject jData)
    {
        JArray Events = (JArray)jData["Resp"];
        //WriteDebugLine("ProcessCalendar # Events " + Events.Count);

        for (int i = 0; i < Events.Count; i++)
        {
            JObject Event = (JObject)Events[i];

            int UserID = 0;
            JObject jUser = (JObject)Event["user"];
            if (jUser != null)
            {
                UserID = (int)jUser["id"];
            }
            User User = await FindUser(UserID);

            string ID = (string)Event["id"];
            string Type = (string)Event["type"];
            bool fFullDay = (bool)Event["fullDay"];
            string sEventBeg = (string)Event["dtstart"];
            string sEventEnd = (string)Event["dtend"];
            DateTime dtEventBeg = UserTime(DateTime.Parse(sEventBeg));
            DateTime dtEventEnd = UserTime(DateTime.Parse(sEventEnd));
            string Summary = (string)Event["summary"];
            string Status = (string)Event["status"];
            if (Type == "shift")
            {
                //WriteDebugLine("Shift ID " + ID + " User ID " + UserID + " " + (User == null ? "not found" : "Found") + " summary " + Summary);
            }
            //WriteDebugLine("Summary " + Summary);
            CalendarEvent CalEvent = new CalendarEvent(ID, Type, fFullDay, dtEventBeg, dtEventEnd, Summary, Status, User);
            if (User != null)
            {
                User.AddCalendarEvent(CalEvent);
            }

            if (CalEvent.JobRno != 0)
            {
                //WriteDebugLine("ProcessCalendar shift ID " + ID + " JobRno " + CalEvent.JobRno);
                AddShift(CalEvent);
                //WriteDebugLine("# shifts " + Shifts.Count.ToString());
            }
        }
    }


    public async Task LoadAvailability(DateTime dtBeg, DateTime dtEnd)
    {
        foreach (User User in Users)
        {
            //WriteDebugLine("User " + User.FirstName + " " + User.LastName);
            await LoadAvailabilitySets(User, dtBeg, dtEnd);
        }
    }

    public async Task LoadAvailabilitySets(User User, DateTime dtBeg, DateTime dtEnd)
    {
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.GetAsync(string.Format("{0}/availability?userId={1}", sBaseUrl, User.ID));

        await DebugHttpCall(string.Empty, Response);

        if (Response.StatusCode == HttpStatusCode.OK)
        {
            string Resp = await Response.Content.ReadAsStringAsync();
            Resp = string.Format("{{\"Resp\":{0}}}", Resp);     // make the data readable for JObject.Parse()
            JObject jData = JObject.Parse(Resp);
            JArray AvailabilitySets = (JArray)jData["Resp"];
            for (int i = 0; i < AvailabilitySets.Count; i++)
            {
                JObject AvailabilitySet = (JObject)AvailabilitySets[i];
                string AvailabilitySetId = (string)AvailabilitySet["id"];
                DateTime dtStart = Str.DtTm((string)AvailabilitySet["start"]);
                DateTime dtUntil = Str.DtTmEnd((string)AvailabilitySet["until"]);
                string Freq = (string)AvailabilitySet["freq"];

                if (dtStart <= dtEnd)
                {
                    await LoadAvailabilitySet(User, AvailabilitySetId, Freq, dtBeg, dtEnd);
                }
            }
        }
    }

    public async Task LoadAvailabilitySet(User User, string AvailabilitySetId, string Freq, DateTime dtBeg, DateTime dtEnd)
    {
        HttpClient Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("Authorization", Authorization);
        HttpResponseMessage Response = await Client.GetAsync(string.Format("{0}availability/{1}", sBaseUrl, AvailabilitySetId));

        await DebugHttpCall(string.Empty, Response);

        if (Response.StatusCode == HttpStatusCode.OK)
        {
            string Resp = await Response.Content.ReadAsStringAsync();
            JObject jData = JObject.Parse(Resp);
            JArray Availabilities = (JArray)jData["availabilities"];
            for (int i = 0; i < Availabilities.Count; i++)
            {
                JObject Event = (JObject)Availabilities[i];

                string ID = (string)Event["id"];
                string Type = (string)Event["type"];
                bool fFullDay = (bool)Event["fullDay"];
                DateTime dtEventBeg = Str.DtTm((string)Event["dtstart"]);
                DateTime dtEventEnd = Str.DtTmEnd((string)Event["dtend"]);
                string Summary = (string)Event["summary"];
                string Status = (string)Event["status"];

                // bring the availability event into the desired date range
                switch (Freq)
                {
                    case "WEEKLY":
                        while (dtEventBeg < dtBeg)
                        {
                            dtEventBeg = dtEventBeg.AddDays(7);
                            dtEventEnd = dtEventEnd.AddDays(7);
                        }
                        break;
                }

                CalendarEvent CalEvent = new CalendarEvent(ID, Type, fFullDay, dtEventBeg, dtEventEnd, Summary, Status, User);
                User.AddCalendarEvent(CalEvent);
            }
        }
    }

    public void AddShift(CalendarEvent Event)
    {
        cShifts.Add(Event);
    }

    public CalendarEvent FindShift(string ShiftId)
    {
        CalendarEvent Found = null;

        //WriteDebugLine("Find Shift # Shifts " + Shifts.Count);
        foreach (CalendarEvent Shift in Shifts)
        {
            if (Shift.ID == ShiftId)
            {
                Found = Shift;
                break;
            }
        }

        return Found;
    }

    public void SortShifts()
    {
        for (int i = 0; i < Shifts.Count - 1; i++)
        {
            for (int j = i + 1; j < Shifts.Count; j++)
            {
                CalendarEvent ShiftA = Shifts[i];
                CalendarEvent ShiftB = Shifts[j];

                int Comp = DateTime.Compare(ShiftA.Beg, ShiftB.Beg);
                if (Comp > 0)
                {
                    Shifts[i] = ShiftB;
                    Shifts[j] = ShiftA;
                }
            }
        }
    }

    public void SortShiftCrew(Collection<CalendarEvent> Crew)
    {
        for (int i = 0; i < Crew.Count - 1; i++)
        {
            for (int j = i + 1; j < Crew.Count; j++)
            {
                CalendarEvent EventA = Crew[i];
                CalendarEvent EventB = Crew[j];
                int Comp = DateTime.Compare(EventA.Beg, EventB.Beg);

                if (Comp == 0)
                {
                    User UserA = EventA.User;
                    User UserB = EventB.User;

                    if (UserA == null && UserB == null)
                    {
                        Comp = 0;
                    }
                    else if (UserA == null)
                    {
                        Comp = -1;
                    }
                    else if (UserB == null)
                    {
                        Comp = 1;
                    }
                    else
                    {
                        Comp = string.Compare(UserA.FirstName.ToLower(), UserB.FirstName.ToLower());
                        if (Comp == 0)
                        {
                            Comp = string.Compare(UserA.LastName.ToLower(), UserB.LastName.ToLower());
                        }
                    }
                }

                if (Comp > 0)
                {
                    Crew[i] = EventB;
                    Crew[j] = EventA;
                }
            }
        }
    }

    public class User
    {
        protected int nID;
        protected string sFirstName;
        protected string sLastName;
        protected string sEmail;
        protected Collection<CalendarEvent> cCalendarEvents;

        public int ID { get { return nID; } }
        public string FirstName { get { return sFirstName; } }
        public string LastName { get { return sLastName; } }
        public string Email { get { return sEmail; } }
        public Collection<CalendarEvent> CalendarEvents { get { return cCalendarEvents; } }

        public User(int ID, string FirstName, string LastName, string Email)
        {
            nID = ID;
            sFirstName = FirstName;
            sLastName = LastName;
            sEmail = Email;

            ClearCalendarEvents();
        }

        public void ClearCalendarEvents()
        {
            cCalendarEvents = new Collection<CalendarEvent>();
        }

        public void AddCalendarEvent(CalendarEvent CalendarEvent)
        {
            cCalendarEvents.Add(CalendarEvent);
        }
    }

    public class Group
    {
        protected int nID;
        protected string sName;
        protected string sType;

        protected Collection<User> cUsers = new Collection<User>();

        public int ID { get { return nID; } }
        public string Name { get { return sName; } }
        public string Type { get { return sType; } }
        public Collection<User> Users { get { return cUsers; } }

        public Group(int ID, string Name, string Type)
        {
            nID   = ID;
            sName = Name;
            sType = Type;

            cUsers = new Collection<User>();
        }
    }

    public class CalendarEvent
    {
		protected string nID;
        protected string sType;
        protected bool fFullDay;
        protected DateTime dtBeg;
        protected DateTime dtEnd;
        protected string sSummary;
        protected string sStatus;
        protected User cUser;
        protected int nJobRno;

        public string ID { get { return nID; } }
        public string Type { get { return sType; } }
        public bool FullDay { get { return fFullDay; } }
        public DateTime Beg { get { return dtBeg; } }
        public DateTime End { get { return dtEnd; } }
        public string Summary { get { return sSummary; } }
        public string Status { get { return sStatus; } }
        public User User { get { return cUser; } }
        public int JobRno { get { return nJobRno; } }

        public CalendarEvent(string ID, string Type, bool fFullDay, DateTime dtBeg, DateTime dtEnd, string Summary, string Status, User User)
        {
            nID = ID;
            sType = Type;
            this.fFullDay = fFullDay;
            this.dtBeg = dtBeg;
            this.dtEnd = dtEnd;
            sSummary = Summary;
            sStatus = Status;
            cUser = User;

            nJobRno = 0;
            if (Summary != null)
            {
                int i = Summary.IndexOf("Job #");
                if (i >= 0)
                {
                    i += 5;
                    int iSpace = Summary.IndexOf(" ", i);
                    if (iSpace < 0)
                    {
                        iSpace = Summary.Length;
                    }
                    int iNL = Summary.IndexOf("\n", i);
                    if (iNL < 0)
                    {
                        iNL = Summary.Length;
                    }
                    nJobRno = Str.Num(Summary.Substring(i, Math.Min(iSpace, iNL) - i));
                }
            }
        }
    }
}