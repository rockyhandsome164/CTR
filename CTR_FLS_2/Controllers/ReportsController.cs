namespace CTR_FLS_2.Controllers
{
    using CTR_FLS_2.Services;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Web;
    using Telerik.Reporting.Cache.File;
    using Telerik.Reporting.Services;
    using Telerik.Reporting.Services.WebApi;

    //The class name determines the service URL. 
    //ReportsController class name defines /api/reports/ service URL.
    public class ReportsController : ReportsControllerBase
    {

        static ReportServiceConfiguration configurationInstance;

        static ReportsController()
        {
            //This is the folder that contains the report definitions
            //In this case this is the Reports folder
            var appPath = HttpContext.Current.Server.MapPath("~/");
            var reportsPath = Path.Combine(appPath, "Reports");
            var tempPath = Path.Combine(appPath, "Temp");

            //Add resolver for trdx/trdp report definitions, 
            //then add resolver for class report definitions as fallback resolver; 
            //finally create the resolver and use it in the ReportServiceConfiguration instance.
            var resolver = new UriReportSourceResolver(reportsPath)
                .AddFallbackResolver(new TypeReportSourceResolver());

            //Setup the ReportServiceConfiguration
            configurationInstance = new ReportServiceConfiguration
            {
                HostAppId = "CTRFLS",
                // Override the default location for the reports work area. Use the Temp path
                // that is within the applications entire work folder.
                // Note that the IIS_IUSRS group should have Write access in the ACL in order
                // to work correctly. Creating a work folder outside of the application folder
                // did not work even though proper permissions were granted. - JAM
                Storage = new FileStorage(tempPath),

                ReportSourceResolver = resolver,
                ReportSharingTimeout = 0,
                ClientSessionTimeout = 15

            };
        }

        public ReportsController()
        {
            try
            {
                //Initialize the service configuration
                this.ReportServiceConfiguration = configurationInstance;

            }
            catch (Exception Ex)
            {
                // Don't use DI in this object since Telerik is using this directly
                // and we don't want to change the signature of the constructor
                ILoggingServices _logger = new LoggingServices();
                _logger.LogError(Ex);
            }
        }
    }
}