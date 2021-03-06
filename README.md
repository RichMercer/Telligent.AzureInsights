# Telligent.AzureInsights
Provides tracking code for Azure Insights for Telligent Community

When deployed in a Telligent Community, in addition to standard web application reporting, it will also log events like user created, thread create etc.
This allows you to view the social activity occuring in your site and chart it alongside server resource usage.

## Installation
1. If you don't already have an Azure account, create one and login to https://portal.azure.com.
2. Create a new Azure Insights application.  
3. Extract the files from the release package to your website (ensure you add it to each web node if you are running more than one web server).
4. ?? Edit the ApplicationInsights.config file and add the following if it doesn't already exist.
`
<InstrumentationKey>[your_instrumentation_key]</InstrumentationKey>
<StatusMonitor>2.0.0</StatusMonitor>
`
5. Configure the 'Azure Insights Tracking Plugin' plugin and add your Instrumentation Key found in the Azure Portal for the application you created.

6. If running on IIS, ensure the IIS AppPool identity is a member of the 'Performance Monitor Users' group to allow it to collect server performance data.

## IIS Install
If you are running your site in IIS then to get all data you will also need to download and run the [Status Monitor installer](http://go.microsoft.com/fwlink/?LinkId=506648) on your server. For more information on installing see the documentation at https://docs.microsoft.com/en-us/azure/application-insights/app-insights-monitor-performance-live-website-now#monitor-a-live-iis-web-app.
