using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telligent.DynamicConfiguration.Components;
using Telligent.Evolution.Components;
using Telligent.Evolution.Extensibility;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using Telligent.Evolution.Extensibility.Urls.Version1;
using Telligent.Evolution.Extensibility.Version1;

namespace Telligent.AzureInsights
{
    public class InsightsTrackingCodePlugin : IPlugin, IHtmlHeaderExtension, IConfigurablePlugin
    {
        #region IPlugin Members

        public string Name => "Azure Insights Tracking Plugin";

        public string Description => "Adds the Azure Insights tracking script to all pages and adds information such as the authenticated user id.";

        public void Initialize()
        {
        }

        #endregion

        #region IHtmlHeaderExtension Members

        public bool IsCacheable => true;

        public bool VaryCacheByUser => true;

        public string GetHeader(RenderTarget target)
        {
            var instrKey = _configuration.GetString("instrumentationKey");
            if (string.IsNullOrEmpty(instrKey))
                return string.Empty;

            var trackingScript = @"<script type=""text/javascript"">  var appInsights=window.appInsights||function(config){{    function r(config){{t[config]=function(){{var i=arguments;t.queue.push(function(){{t[config].apply(t,i)}})}}}}var t={{config:config}},u=document,e=window,o=""script"",s=u.createElement(o),i,f;for(s.src=config.url||""//az416426.vo.msecnd.net/scripts/a/ai.0.js"",u.getElementsByTagName(o)[0].parentNode.appendChild(s),t.cookie=u.cookie,t.queue=[],i=[""Event"",""Exception"",""Metric"",""PageView"",""Trace""];i.length;)r(""track""+i.pop());return r(""setAuthenticatedUserContext""),r(""clearAuthenticatedUserContext""),config.disableExceptionTracking||(i=""onerror"",r(""_""+i),f=e[i],e[i]=function(config,r,u,e,o){{var s=f&&f(config,r,u,e,o);return s!==!0&&t[""_""+i](config,r,u,e,o),s}}),t    }}({{        instrumentationKey:""{0}""    }});           window.appInsights=appInsights;    appInsights.trackPageView(); {1}</script>";
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

        IPluginConfiguration _configuration;

        public DynamicConfiguration.Components.PropertyGroup[] ConfigurationOptions
        {
            get
            {
                var groups = new[] { new PropertyGroup("options", "Options", 0) };
                groups[0].Properties.Add(new Property("instrumentationKey", "Instrumentation Key", PropertyType.String, 0, string.Empty));
                return groups;
            }
        }

        public void Update(IPluginConfiguration configuration)
        {
            _configuration = configuration;
        }

        #endregion
    }
}
