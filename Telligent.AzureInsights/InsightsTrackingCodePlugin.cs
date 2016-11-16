using System;
using System.Runtime.ExceptionServices;
using Microsoft.ApplicationInsights;
using Telligent.DynamicConfiguration.Components;
using Telligent.Evolution.Extensibility;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using Telligent.Evolution.Extensibility.Version1;

namespace Telligent.AzureInsights
{
    public class InsightsTrackingCodePlugin : IHtmlHeaderExtension, IConfigurablePlugin
    {
        private static TelemetryClient _telemetry;
        private IUsers _usersService;
        private IGroups _groupsService;
        private IForums _forumsService;
        private IForumThreads _threadsService;
        private IForumReplies _repliesService;


        #region Properties

        public static string InstrumentationKey => Configuration.GetString("instrumentationKey");

        #endregion

        #region IPlugin Members

        public string Name => "Azure Insights Tracking Plugin";

        public string Description => "Adds the Azure Insights tracking script to all pages and adds information such as the authenticated user id.";

        public void Initialize()
        {
            _usersService = Apis.Get<IUsers>();
            _groupsService = Apis.Get<IGroups>();
            _forumsService = Apis.Get<IForums>();
            _threadsService = Apis.Get<IForumThreads>();
            _repliesService = Apis.Get<IForumReplies>();

            _telemetry = new TelemetryClient {Context = {InstrumentationKey = InstrumentationKey}};

            _usersService.Events.AfterCreate += args => LogInsightsEvent("UserCreated");
            _groupsService.Events.AfterCreate += args => LogInsightsEvent("GroupCreated");
            _forumsService.Events.AfterCreate += args => LogInsightsEvent("ForumCreated");
            _threadsService.Events.AfterCreate += args => LogInsightsEvent("ForumThreadCreated");
            _repliesService.Events.AfterCreate += args => LogInsightsEvent("ForumReplyCreated");

            Apis.Get<IUsers>().Events.AfterIdentify += EventsOnAfterIdentify;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void EventsOnAfterIdentify(UserAfterIdentifyEventArgs e)
        {
            if (e.Username != _usersService.AnonymousUserName)
            {
                _telemetry.Context.User.AuthenticatedUserId = e.Id.ToString();
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                // Handle any exceptions here to prevent recursive exceptions which will terminate the application.
                _telemetry.TrackException((Exception) unhandledExceptionEventArgs.ExceptionObject);
            }
            catch { }
        }
        
        #endregion

        #region IHtmlHeaderExtension Members

        public bool IsCacheable => true;

        public bool VaryCacheByUser => true;

        public string GetHeader(RenderTarget target)
        {
            var instrKey = InstrumentationKey;
            if (string.IsNullOrEmpty(instrKey))
                return string.Empty;

            var trackingScript = @"<script type=""text/javascript"">  var appInsights=window.appInsights||function(config){{ function r(config){{t[config]=function(){{var i=arguments;t.queue.push(function(){{t[config].apply(t,i)}})}}}}var t={{config:config}},u=document,e=window,o=""script"",s=u.createElement(o),i,f;for(s.src=config.url||""//az416426.vo.msecnd.net/scripts/a/ai.0.js"",u.getElementsByTagName(o)[0].parentNode.appendChild(s),t.cookie=u.cookie,t.queue=[],i=[""Event"",""Exception"",""Metric"",""PageView"",""Trace""];i.length;)r(""track""+i.pop());return r(""setAuthenticatedUserContext""),r(""clearAuthenticatedUserContext""),config.disableExceptionTracking||(i=""onerror"",r(""_""+i),f=e[i],e[i]=function(config,r,u,e,o){{var s=f&&f(config,r,u,e,o);return s!==!0&&t[""_""+i](config,r,u,e,o),s}}),t    }}({{        instrumentationKey:""{0}""    }});           window.appInsights=appInsights;    appInsights.trackPageView(); {1}</script>";
            var userTracking = string.Empty;

            var currentUser = Apis.Get<IUsers>().AccessingUser;
            if (!string.IsNullOrEmpty(currentUser?.Username))
            {
                userTracking = $"appInsights.setAuthenticatedUserContext('{currentUser.Username}');";
            }

            return string.Format(trackingScript, instrKey, userTracking);
        }

        #endregion

        #region IConfigurablePlugin Members

        public static IPluginConfiguration Configuration;

        public PropertyGroup[] ConfigurationOptions
        {
            get
            {
                var groups = new[] { new PropertyGroup("options", "Options", 0) };
                groups[0].Properties.Add(new Property("instrumentationKey", "Instrumentation Key", PropertyType.String, 0, string.Empty));
                groups[0].Properties.Add(new Property("logEvents", "Log Events", PropertyType.Bool, 1, false.ToString()));
                return groups;
            }
        }

        public void Update(IPluginConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        private void LogInsightsEvent(string eventName)
        {
            if (!Configuration.GetBool("logEvents"))
                return;

            try
            {
                _telemetry.TrackEvent(eventName);
            }
            catch (Exception ex)
            {
                new EventLog().Write($"Unable to log Azure Insights Event. {ex.Message}",
                    new EventLogEntryWriteOptions { Category = "Azure Insights", EventType = "Error" });
            }
        }
    }
}
