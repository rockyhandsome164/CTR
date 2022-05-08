using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Hyland.Unity;
using Hyland.Types;
using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System.IO;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using System.Diagnostics;

namespace CTR_FLS_2.Services
{
    public class OnBaseServices : IOnBaseServices
    {
        private readonly ILoggingServices LoggingServ;
        private readonly ICommonServices CommonServ;
        private readonly IJobTreeServices JobTreeServ;

        private Application UnityApp = null;

        public OnBaseServices() : this(new LoggingServices(),
                                       new CommonServices(),
                                       new JobTreeServices()
                                       ) 
        { }

        public OnBaseServices(ILoggingServices LoggingServicesObj,
                              ICommonServices CommonServicesObj,
                              IJobTreeServices JobTreeServicesObj)
        {
            LoggingServ = LoggingServicesObj;
            CommonServ = CommonServicesObj;
            JobTreeServ = JobTreeServicesObj;
        }

        /// <summary>
        /// Gets a list of document meta data from an OnBase query
        /// </summary>
        /// <returns></returns>
        public OnBaseViewModel GetOnBaseDocInfo(string DocType, List<OnBaseQueryCriteria> QueryCriteria, string DocOutputFileName, string DocOutputType)
        {
            OnBaseViewModel OnBaseVM = new OnBaseViewModel();
            OnBaseVM.UserMessage = "OK";
            string OnBaseFilesFolderName = "~/Reports/OnBaseFiles/";
            string OnBaseFileName = "";

            List<OnBaseDocMeta> DocMetaData = new List<OnBaseDocMeta>();

            // If we come in with no predefined file name, then this call is coming from a request to view an Onbase file
            // so we will create a dynamic file name to store it with. 
            // If we do have a file name, then it means we're coming in here from creating a package (from the create pkg method below)
            //bool CreatingOnBaseFiles = string.IsNullOrWhiteSpace(DocOutputFileName);

            try
            {
                // Establish a connection
                UnityApp = GetUnityAppObject();

                using(UnityApp)
                {
                    // Get a doc query obj
                    DocumentQuery DocQryObj = GetDocumentQueryObject(DocType);

                    // Add keywords
                    foreach (OnBaseQueryCriteria KWCriteria in QueryCriteria)
                    {
                        KeywordType KWType = UnityApp.Core.KeywordTypes.Find(KWCriteria.KeywordType);
                        if (KWType != null)
                        {
                            Hyland.Unity.Keyword KW = KWType.CreateKeyword(KWCriteria.KeywordValue);
                            DocQryObj.AddKeyword(KW);

                            if (DocOutputType.Equals(Constants.ONBASE_DOC_OUTPUT_TYPE_FILE) && string.IsNullOrWhiteSpace(DocOutputFileName))
                            {
                                OnBaseFileName = KWCriteria.KeywordValue + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                                DocOutputFileName = Path.Combine(HttpContext.Current.Server.MapPath(OnBaseFilesFolderName), OnBaseFileName);
                            }
                        }
                        else
                        {
                            throw new Exception(KWCriteria.KeywordType + " is not valid");
                        }
                    }

                    // Go fetch. . . good dog...
                    using (QueryResult QryRslt = DocQryObj.ExecuteQueryResults(1))
                    {
                        if (QryRslt.QueryResultItems.Count() == 0)
                        {
                            OnBaseVM.UserMessage = Constants.ONBASE_NO_DOCS_FOUND_MSG;
                        }
                        else
                        {
                            foreach (QueryResultItem QryRsltItem in QryRslt.QueryResultItems)
                            {
                                Document CurrentDoc = QryRsltItem.Document;

                                // Get the file image for display in the UI
                                // array to hold the image stream
                                byte[] DocImgByteArray = null;
                                string DocImgBase64Encoded = "";

                                // Make sure doc can be viewed
                                if (CurrentDoc.DocumentType.CanI(DocumentTypePrivileges.DocumentViewing))
                                {
                                    // Pull down a PDF version of the doc 
                                    // Note that other rendering options are available (e.g. tiff)
                                    Rendition rendition = CurrentDoc.DefaultRenditionOfLatestRevision;
                                    PDFDataProvider PDFProvider = UnityApp.Core.Retrieval.PDF;
                                    PDFGetDocumentProperties PDFDocumentProps = PDFProvider.CreatePDFGetDocumentProperties();
                                    PDFDocumentProps.Overlay = false;
                                    PDFDocumentProps.OverlayAllPages = false;
                                    PDFDocumentProps.RenderNoteAnnotations = false;
                                    PDFDocumentProps.RenderNoteText = false;

                                    using (PageData pageData = PDFProvider.GetDocument(rendition, PDFDocumentProps))
                                    {
                                        // *** This streaming section is not working consistently when trying
                                        // *** render the PDF through the UI so this part of the IF statement
                                        // *** will not hit. Keeping the code as is in case we want to revisit this
                                        // *** in the future

                                        // If we don't come in with a file name, then we will be sending 
                                        // the file stream back to the UI for display
                                        switch (DocOutputType)
                                        {
                                            case Constants.ONBASE_DOC_OUTPUT_TYPE_STREAM :

                                                // Convert the downloaded stream to a byte array
                                                using (MemoryStream MS = new MemoryStream())
                                                {
                                                    pageData.Stream.CopyTo(MS);
                                                    DocImgByteArray = MS.ToArray();
                                                }

                                                // Encoded it so we can make it available as a link in the UI
                                                DocImgBase64Encoded = Convert.ToBase64String(DocImgByteArray);

                                                break;

                                            case Constants.ONBASE_DOC_OUTPUT_TYPE_FILE:
                                                Utility.WriteStreamToFile(pageData.Stream, $@"{ DocOutputFileName}");

                                                DocOutputFileName = OnBaseFilesFolderName.Replace("~", "..") + "/" + OnBaseFileName;

                                                break;

                                            case Constants.ONBASE_DOC_OUTPUT_TYPE_EXISTANCE:
                                                break;
                                        }

                                        OnBaseVM.UserMessage = "OK";
                                    }
                                }

                                DocMetaData.Add(new OnBaseDocMeta
                                {
                                    DocumentName = DocOutputFileName,
                                    DocumentDisplayName = CurrentDoc.Name,
                                    DocumentType = CurrentDoc.DocumentType.Name,
                                    DocumentDate = CurrentDoc.DocumentDate,
                                    DocumentId = CurrentDoc.ID,
                                    DocumentImg = DocImgBase64Encoded
                                });
                            }
                        }
                    }

                    OnBaseVM.DocMetaData = DocMetaData;
                }
            }
            catch (Exception Ex)
            {
                LoggingServ.LogError(Ex);
                OnBaseVM.UserMessage = Ex.Message;
                if (UnityApp != null)
                {
                    if (UnityApp.IsConnected)
                    {
                        UnityApp.Disconnect();
                    }
                    UnityApp.Dispose();
                }
            }

            return OnBaseVM;
        }

        /// <summary>
        /// Saves a CTR physically to the folder in PDF format.
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="LotNbr"></param>
        /// <returns></returns>
        public string CTRPDFExport(string FileName, string LotNbr)
        {
            string ReturnValue = "";

            try
            {
                ReportProcessor ReportProcessor = new ReportProcessor();
                Hashtable DeviceInfo = new Hashtable();
                UriReportSource UriReportSource = new UriReportSource();
                UriReportSource.Uri = @"Reports\ReportObjectsInTRDXFormat\" + FileName;
                UriReportSource.Parameters.Add("LotNbr", LotNbr);
                UriReportSource.Parameters.Add("ShipperNbr", "");
                UriReportSource.Parameters.Add("ConnectionString", System.Configuration.ConfigurationManager.ConnectionStrings["CTR_FLS"].ConnectionString);

                RenderingResult Result = ReportProcessor.RenderReport("PDF", UriReportSource, DeviceInfo);
                if (!Result.HasErrors)
                {
                    string CTRReportPath = HttpContext.Current.Server.MapPath("~/Reports/CTRReports/");
                    string TRDXFileNameWithoutExtension = FileName.Substring(0, FileName.LastIndexOf("_"));
                    string FullFileName = "99_" + TRDXFileNameWithoutExtension + "." + Result.Extension;

                    using (FileStream FS = new FileStream(Path.Combine(CTRReportPath, FullFileName), FileMode.Create))
                    {
                        FS.Write(Result.DocumentBytes, 0, Result.DocumentBytes.Length);
                    }
                    ReturnValue = FullFileName;
                }
            }
            catch(Exception e)
            {
                ReturnValue = "Error in exporting CTR to PDF";
            }

            return ReturnValue;
        }

        /// <summary>
        /// Saves a file to OnBase using key words
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="DocType"></param>
        /// <param name="LotNbr"></param>
        /// <returns></returns>
        public OnBaseViewModel SaveCTRDocToOnBase(string FileName, string DocType, string LotNbr)
        {
            OnBaseViewModel OnBaseVM = new OnBaseViewModel();

            try
            {
                // Check to see if this CTR exists in OnBase.
                // If so, we will save as a revision rather than new entry
                bool HaveExistingOnBaseDoc = false;
                List<OnBaseQueryCriteria> QryCriteria = new List<OnBaseQueryCriteria>();
                QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = Constants.ONBASE_KEYWORD_TYPE_LOT_NBR, KeywordOperator = "=", KeywordValue = "9-" + LotNbr });

                OnBaseViewModel OnBaseVMForCheckingExistance = GetOnBaseDocInfo(Constants.ONBASE_DOC_TYPE_TEST_REPORT, QryCriteria, null, Constants.ONBASE_DOC_OUTPUT_TYPE_EXISTANCE);
                if (OnBaseVMForCheckingExistance != null)
                {
                    if (OnBaseVMForCheckingExistance.UserMessage.Equals("OK"))
                    {
                        HaveExistingOnBaseDoc = true;
                    }
                }

                UnityApp = GetUnityAppObject();

                using (UnityApp)
                {
                    DocumentType DocumentType = UnityApp.Core.DocumentTypes.Find(DocType);
                    if (DocumentType == null)
                    {
                        throw new Exception("Document Type is not valid.");
                    }

                    FileType FileType = UnityApp.Core.FileTypes.Find("PDF");
                    if (FileType == null)
                    {
                        throw new Exception("File Type is not valid.");
                    }

                    if (DocumentType.CanI(DocumentTypePrivileges.DocumentCreation))
                    {
                        Storage Storage = UnityApp.Core.Storage;
                        StoreNewDocumentProperties StoreNewDocumentProperties = Storage.CreateStoreNewDocumentProperties(DocumentType, FileType);


                        // Add keywords
                        Dictionary<string, string> DocKeyWords = GetCTRKeywordsAndValues(LotNbr);
                        foreach(KeyValuePair<string, string> KeyWrd in DocKeyWords)
                        {
                            if (KeyWrd.Key.Equals("Certification Date"))
                            {
                                StoreNewDocumentProperties.AddKeyword(KeyWrd.Key, DateTime.Parse(KeyWrd.Value));
                            }
                            else
                            {
                                StoreNewDocumentProperties.AddKeyword(KeyWrd.Key, KeyWrd.Value);
                            }
                        }

                        // Add keywords for material lots
                        List<string> MaterialLots = GetMaterialLots(LotNbr);
                        foreach (string MatLot in MaterialLots)
                        {
                            StoreNewDocumentProperties.AddKeyword(Constants.ONBASE_KEYWORD_TYPE_MATERIAL_LOT_NBR, MatLot);
                        }

                        // get the path to the PDF
                        string CTRReportPath = HttpContext.Current.Server.MapPath("~/Reports/CTRReports/");

                        // Store the doc in Onbase
                        using (PageData PageData = Storage.CreatePageData(Path.Combine(CTRReportPath, FileName)))
                        {
                            if (HaveExistingOnBaseDoc)
                            {
                                StoreRevisionProperties RevisionProps = Storage.CreateStoreRevisionProperties(UnityApp.Core.GetDocumentByID(OnBaseVMForCheckingExistance.DocMetaData[0].DocumentId), FileType);
                                RevisionProps.Comment = "Revised";
                                Hyland.Unity.Document newDocument = Storage.StoreNewRevision(PageData, RevisionProps);
                            }
                            else
                            {
                                Hyland.Unity.Document newDocument = Storage.StoreNewDocument(PageData, StoreNewDocumentProperties);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Access to archive documents is denied. Please check the permissions for the API account.");
                    }

                    UnityApp.Disconnect();

                    OnBaseVM.UserMessage = FileName + " successfully archived to OnBase.";
                }
            }
            catch (Exception e)
            {
                OnBaseVM.UserMessage = e.Message ?? e.InnerException.Message;
                if (UnityApp != null)
                {
                    if (UnityApp.IsConnected)
                    {
                        UnityApp.Disconnect();
                    }
                    UnityApp.Dispose();
                }
            }

            return OnBaseVM;
        }

        /// <summary>
        /// Note this process requires AppendPDF to be installed on where this is being run (e.g. local dev machine or web server)
        /// THIS IS JUST A POC. Locations of the AppendPDF program, etc. should be put into the config and not hard coded here
        /// </summary>
        /// <param name="LotNbr"></param>
        /// <returns></returns>
        public OnBaseViewModel SaveCertPackageToOnBase(string LotNbr, string ShipperNbr)
        {
            OnBaseViewModel OnBaseVM = new OnBaseViewModel();
            OnBaseViewModel OnBaseVMForPkg = new OnBaseViewModel();
            List<OnBaseQueryCriteria> QryCriteria = new List<OnBaseQueryCriteria>();

            string FileNameForPkg = "";
            string FileNameForCTR = "";
            string FileNameForAppendPDFParms = "";

            // Check to see if this package exists in OnBase.
            // If so, we will save as a revision rather than new entry
            bool HaveExistingOnBaseDoc = false;
            QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = Constants.ONBASE_KEYWORD_TYPE_LOT_NBR, KeywordOperator = "=", KeywordValue = "9-" + LotNbr });

            OnBaseViewModel OnBaseVMForCheckingExistance = GetOnBaseDocInfo(Constants.ONBASE_DOC_TYPE_CERT_PKG, QryCriteria, null, Constants.ONBASE_DOC_OUTPUT_TYPE_EXISTANCE);
            if (OnBaseVMForCheckingExistance != null)
            {
                if (OnBaseVMForCheckingExistance.UserMessage.Equals("OK"))
                {
                    HaveExistingOnBaseDoc = true;
                }
            }


            // Create Stamp file for watermarking
            // Get the Cert
            string CertNbr = "";
            using (CTR_FLS_Entities CTR_FLS = new CTR_FLS_Entities())
            {
                //use this existing proc that pulls in data for the CTR
                //It has the cert nbr plus other stuff
               var CTRInfo = CTR_FLS.GetCTRFormHeaderFooterFields(LotNbr);
                foreach (var Rec in CTRInfo)
                {
                    CertNbr = Rec.CertNbr.ToString();
                }
            }
            string AppendPDFXMLFileContent = "<appendparam version=\"1.0\">";
                                             
            try
            {

                // Pull the job tree to get all docs that need to be included in package
                JobTreeViewModel JobTreeVM = JobTreeServ.GetJobTreeVM(LotNbr);

                // Create a temp directory that will contain the physical files
                string FolderName = "~/Reports/CertPackages/" + LotNbr + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                string TempWorkingFolder = HttpContext.Current.Server.MapPath(FolderName);
                Directory.CreateDirectory(TempWorkingFolder);

                string StampFileName = "9-" + LotNbr + "_PKG_STAMP.txt";
                string StampFileFullPath = Path.Combine(TempWorkingFolder, StampFileName);
                AddWatermarkToCTRPDF(StampFileFullPath, ShipperNbr, CertNbr);


                // First set the CTR package for the lot nbr entered
                FileNameForPkg = LotNbr + "_CERT_PKG.pdf";
                FileNameForCTR = LotNbr + "_CTR.pdf";

                // Set the output file XML block for appendPDF
                FileNameForAppendPDFParms = LotNbr + "_Parms.xml";
                AppendPDFXMLFileContent += "<outputpdf>" +
                                           "<file>" + Path.Combine(TempWorkingFolder, FileNameForPkg) + "</file>" +
                                           "</outputpdf>";

                QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = Constants.ONBASE_KEYWORD_TYPE_LOT_NBR, KeywordOperator = "=", KeywordValue = "9-" + LotNbr });

                // Get the CTR report
                OnBaseVM = GetOnBaseDocInfo(Constants.ONBASE_DOC_TYPE_TEST_REPORT, QryCriteria, Path.Combine(TempWorkingFolder, FileNameForCTR), Constants.ONBASE_DOC_OUTPUT_TYPE_FILE);
                if (!OnBaseVM.UserMessage.Equals(Constants.ONBASE_NO_DOCS_FOUND_MSG))
                {

                    AppendPDFXMLFileContent += "<sourcepdfs>";

                    // Add the CTR file to the package
                    AppendPDFXMLFileContent += "<inputpdf>" +
                                               "<file>" + Path.Combine(TempWorkingFolder, FileNameForCTR) + "</file>" +
                                               "<startpage>1</startpage>" +
                                               "<endpage>-1</endpage>" +
                                               "</inputpdf>";

                    // Now loop thru job tree and pull out only Material Certs and OSPs
                    foreach (JobTreeItem JTItem in JobTreeVM.JobTreeItems)
                    {
                        QryCriteria.Clear();

                        // Pull material doc from Onbase and save into temp dir
                        if (!String.IsNullOrWhiteSpace(JTItem.MaterialLot))
                        {
                            string FileNameForMatCert = JTItem.MaterialLot + "_MAT_CERT.pdf";
                            QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = Constants.ONBASE_KEYWORD_TYPE_MATERIAL_LOT_NBR, KeywordOperator = "=", KeywordValue = JTItem.MaterialLot});

                            OnBaseVM = GetOnBaseDocInfo(Constants.ONBASE_DOC_TYPE_MATERIAL_CERT, QryCriteria, Path.Combine(TempWorkingFolder, FileNameForMatCert), Constants.ONBASE_DOC_OUTPUT_TYPE_FILE);

                            // Build XML block for AppendPDF service which will do the merging of the physical PDF docs into a single file
                            AppendPDFXMLFileContent += "<inputpdf>" +
                                                       "<file>" + Path.Combine(TempWorkingFolder, FileNameForMatCert) + "</file>" +
                                                       "<startpage>1</startpage>" +
                                                       "<endpage>-1</endpage>" +
                                                       "</inputpdf>";
                        }
                    }

                    // Finally see if there are any OSPs for the top level component ONLY!!
                    List<OSP> OspRecs = CommonServ.GetOSP(LotNbr);
                    // Could be multiple
                    int OSPFileIterator = 0;
                    foreach (OSP OSPRec in OspRecs)
                    {
                        if (OSPRec.AttachmentType.Equals("SurfTreat"))
                        {
                            OSPFileIterator++;
                            string FileNameForOSP = LotNbr + "_OSP_CERT_" + OSPFileIterator.ToString() + ".pdf";
                            QryCriteria.Add(new OnBaseQueryCriteria { KeywordType = Constants.ONBASE_KEYWORD_TYPE_LOT_NBR, KeywordOperator = "=", KeywordValue = LotNbr });

                            OnBaseVM = GetOnBaseDocInfo(Constants.ONBASE_DOC_TYPE_OSP_CERT, QryCriteria, Path.Combine(TempWorkingFolder, FileNameForOSP), Constants.ONBASE_DOC_OUTPUT_TYPE_FILE);

                            // Build XML block for AppendPDF service which will do the merging of the physical PDF docs into a single file
                            AppendPDFXMLFileContent += "<inputpdf>" +
                                                       "<file>" + Path.Combine(TempWorkingFolder, FileNameForOSP) + "</file>" +
                                                       "<startpage>1</startpage>" +
                                                       "<endpage>-1</endpage>" +
                                                       "</inputpdf>";
                        }
                    }

                    // Save XML file
                    AppendPDFXMLFileContent += "</sourcepdfs>";

                    // Include the stamp file
                    AppendPDFXMLFileContent += "<extras>";
                    AppendPDFXMLFileContent += "<stampfile>";
                    AppendPDFXMLFileContent += StampFileFullPath;
                    AppendPDFXMLFileContent += " </stampfile>";

                    AppendPDFXMLFileContent += "</extras>";

                    AppendPDFXMLFileContent += "</appendparam>";
                    File.WriteAllText(Path.Combine(TempWorkingFolder, FileNameForAppendPDFParms), AppendPDFXMLFileContent);

                    // Shell out to run AppendPDF
                    // NOTE WHEN DEPLOYED TO THE WEBSERVER, MAKE SURE APP POOL ACCOUNT HAS R/W/E PERMISSIONS
                    string ExecCmd = @"C:\Appligent\AppendPro\appendpro " + Path.Combine(TempWorkingFolder, FileNameForAppendPDFParms);

                    var AppendPDFExec = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C " + ExecCmd,
                        WindowStyle = ProcessWindowStyle.Normal,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    Process.Start(AppendPDFExec);


                    // Save newly created package file to Onbase
                    UnityApp = GetUnityAppObject();

                    using (UnityApp)
                    {
                        DocumentType DocumentType = UnityApp.Core.DocumentTypes.Find(Constants.ONBASE_DOC_TYPE_CERT_PKG);
                        if (DocumentType == null)
                        {
                            throw new Exception("Document Type is not valid.");
                        }

                        FileType FileType = UnityApp.Core.FileTypes.Find("PDF");
                        if (FileType == null)
                        {
                            throw new Exception("File Type is not valid.");
                        }

                        if (DocumentType.CanI(DocumentTypePrivileges.DocumentCreation))
                        {
                            Storage Storage = UnityApp.Core.Storage;
                            StoreNewDocumentProperties StoreNewDocumentProperties = Storage.CreateStoreNewDocumentProperties(DocumentType, FileType);
                            StoreNewDocumentProperties.AddKeyword("Lot Number", "9-" + LotNbr);
                            StoreNewDocumentProperties.AddKeyword("Site", "FLS");
                            StoreNewDocumentProperties.AddKeyword("Certificate Number", CertNbr);
                            StoreNewDocumentProperties.AddKeyword("Shipper Number", ShipperNbr);

                            // Store the doc in Onbase
                            using (PageData PageData = Storage.CreatePageData(Path.Combine(TempWorkingFolder, FileNameForPkg)))
                            {
                                if (HaveExistingOnBaseDoc)
                                {
                                    StoreRevisionProperties RevisionProps = Storage.CreateStoreRevisionProperties(UnityApp.Core.GetDocumentByID(OnBaseVMForCheckingExistance.DocMetaData[0].DocumentId), FileType);
                                    RevisionProps.Comment = "Revised";
                                    Hyland.Unity.Document newDocument = Storage.StoreNewRevision(PageData, RevisionProps);
                                }
                                else
                                {
                                    Hyland.Unity.Document newDocument = Storage.StoreNewDocument(PageData, StoreNewDocumentProperties);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Access to archive documents is denied. Please check the permissions for the API account.");
                        }

                        UnityApp.Disconnect();

                        string FullPath = Path.Combine(TempWorkingFolder, FileNameForPkg);
                        var FileImg = File.ReadAllBytes(Path.Combine(TempWorkingFolder, FileNameForPkg));


                        OnBaseVMForPkg.DocMetaData = new List<OnBaseDocMeta>();
                        OnBaseVMForPkg.DocMetaData.Add(new OnBaseDocMeta
                        {
                            // Format this way as the browser will open a new tab to view the file
                            DocumentName = FolderName.Replace("~", "..") + "/" + FileNameForPkg

                            // *** Couldn't get the streaming of the PDF to work in the browser
                            // *** but left the code in here to try. Getting 500 error because
                            // *** of maxJsonLength not being set large enough though
                            // *** it's set in the controller AND web.config. Tired of playing with it...
                            //DocumentImg = Convert.ToBase64String(FileImg)
                        });

                        OnBaseVMForPkg.UserMessage = FileNameForPkg + " successfully archived to OnBase.";
                    }
                }
                else
                {
                    OnBaseVMForPkg.UserMessage = "Not all documents can be found in OnBase";
                }
            }
            catch (Exception e)
            {
                if (UnityApp != null)
                {
                    if (UnityApp.IsConnected)
                    {
                        UnityApp.Disconnect();
                    }
                    UnityApp.Dispose();
                }
                LoggingServ.LogError(e);
                OnBaseVMForPkg.UserMessage = e.Message;
            }

            return OnBaseVMForPkg;
        }

        public string AddWatermarkToCTRPDF(string CTRFileName, string ShipperNbr, string CertNbr)
        {
            string NewCTRFileName = "";

            string StampFile = "";

            StampFile += "Begin_Options  " + Environment.NewLine;
            StampFile += "Version(2) " + Environment.NewLine;
            StampFile += "End_Options " + Environment.NewLine;

            // This is the date time stamp in the upper left corner
            // Note that the 'Top' is relative to the bottom corner of the form and goes up.
            StampFile += "Begin_Message  " + Environment.NewLine;
            StampFile += "Name          (CreateTimeStamp)   " + Environment.NewLine;
            StampFile += "PageIncrement  (1) " + Environment.NewLine;
            StampFile += "StartPage          (1)   " + Environment.NewLine;
            StampFile += "EndPage          (-1)   " + Environment.NewLine;
            StampFile += "Type          (Text)   " + Environment.NewLine;
            StampFile += "Top          (785)   " + Environment.NewLine;
            StampFile += "Left          (8)   " + Environment.NewLine;
            StampFile += "Position          (Top)   " + Environment.NewLine;
            StampFile += "Justification          (Left)   " + Environment.NewLine;
            StampFile += "Size          (9)   " + Environment.NewLine;
            StampFile += "Text          (Date Created: %c)   " + Environment.NewLine;
            StampFile += "TextMode      (0)  " + Environment.NewLine;

            StampFile += "End_Message " + Environment.NewLine;

            // This is the shipper in the upper top center
            StampFile += "Begin_Message  " + Environment.NewLine;
            StampFile += "Name          (ShipperNbrTopStamp)   " + Environment.NewLine;
            StampFile += "PageIncrement  (1) " + Environment.NewLine;
            StampFile += "StartPage          (1)   " + Environment.NewLine;
            StampFile += "EndPage          (-1)   " + Environment.NewLine;
            StampFile += "Type          (Text)   " + Environment.NewLine;
            StampFile += "Top          (785)   " + Environment.NewLine;
            StampFile += "Left          (15)   " + Environment.NewLine;
            StampFile += "Position          (Top)   " + Environment.NewLine;
            StampFile += "Justification          (Center)   " + Environment.NewLine;
            StampFile += "Size          (9)   " + Environment.NewLine;
            StampFile += "Text          (Shipper# " + ShipperNbr + ")   " + Environment.NewLine;
            StampFile += "TextMode      (0)  " + Environment.NewLine;

            StampFile += "End_Message " + Environment.NewLine;

            // This is the cert stamp in the upper right corner
            StampFile += "Begin_Message  " + Environment.NewLine;
            StampFile += "Name          (CertStamp)   " + Environment.NewLine;
            StampFile += "PageIncrement  (1) " + Environment.NewLine;
            StampFile += "StartPage          (1)   " + Environment.NewLine;
            StampFile += "EndPage          (-1)   " + Environment.NewLine;
            StampFile += "Type          (Text)   " + Environment.NewLine;
            StampFile += "Top          (785)   " + Environment.NewLine;
            StampFile += "Left          (50)   " + Environment.NewLine;
            StampFile += "Position          (Top)   " + Environment.NewLine;
            StampFile += "Justification          (Right)   " + Environment.NewLine;
            StampFile += "Size          (9)   " + Environment.NewLine;
            StampFile += "Text          (Cert# " + CertNbr + ")   " + Environment.NewLine;
            StampFile += "TextMode      (0)  " + Environment.NewLine;

            StampFile += "End_Message " + Environment.NewLine;


            // This is the do not reproduce message that runs vertically along the left side
            StampFile += "Begin_Message  " + Environment.NewLine;
            StampFile += "Name          (ReproduceStamp)   " + Environment.NewLine;
            StampFile += "PageIncrement  (1) " + Environment.NewLine;
            StampFile += "StartPage          (1)   " + Environment.NewLine;
            StampFile += "EndPage          (-1)   " + Environment.NewLine;
            StampFile += "Type          (Text)   " + Environment.NewLine;
            StampFile += "Top          (600)   " + Environment.NewLine;
            StampFile += "Left          (6)   " + Environment.NewLine;
            StampFile += "Position          (TopDown-Left)   " + Environment.NewLine;
            StampFile += "Justification          (left)   " + Environment.NewLine;
            StampFile += "Size          (9)   " + Environment.NewLine;
            StampFile += "Text          (This document shall not be reproduced, except in full, without the written approval of HFS.)   " + Environment.NewLine;
            StampFile += "TextMode      (0)  " + Environment.NewLine;

            StampFile += "End_Message " + Environment.NewLine;

            // This is the shipper number info on the bottom right
            StampFile += "Begin_Message  " + Environment.NewLine;
            StampFile += "Name          (ShipperNbrBottomStamp)   " + Environment.NewLine;
            StampFile += "PageIncrement  (1) " + Environment.NewLine;
            StampFile += "StartPage          (1)   " + Environment.NewLine;
            StampFile += "EndPage          (-1)   " + Environment.NewLine;
            StampFile += "Type          (Text)   " + Environment.NewLine;
            StampFile += "Top          (12)   " + Environment.NewLine;
            StampFile += "Left          (18)   " + Environment.NewLine;
            StampFile += "Justification          (Center)   " + Environment.NewLine;
            StampFile += "Size          (9)   " + Environment.NewLine;
            StampFile += "Text          (Shipper# " + ShipperNbr + ")   " + Environment.NewLine;
            StampFile += "TextMode      (0)  " + Environment.NewLine;

            StampFile += "End_Message " + Environment.NewLine;

            // Save file
            File.WriteAllText(CTRFileName, StampFile);

            return NewCTRFileName;
        }

        private bool CheckForExistingVersionInOnBase()
        {
            bool DocExistsInOnBase = false;

            return DocExistsInOnBase;
        }
        private Dictionary<string, string> GetCTRKeywordsAndValues(string LotNbr)
        {
            Dictionary<string, string> OnBaseKeyWords = new Dictionary<string, string>();

            JobInfo JobInfoRec = CommonServ.GetJobInfoForLot(LotNbr);

            OnBaseKeyWords.Add("Site", "FLS");
            OnBaseKeyWords.Add("Certificate Number", "CTR2.0-" + LotNbr);
            OnBaseKeyWords.Add("Lot Number", "9-" + LotNbr);
            OnBaseKeyWords.Add("Certification Date", DateTime.Now.ToString("MM/dd/yyyy"));
            OnBaseKeyWords.Add("Part Number", JobInfoRec.Item);
            OnBaseKeyWords.Add("Part Description", JobInfoRec.ProductFamily);
            OnBaseKeyWords.Add("Part Revision", JobInfoRec.Revision);
            OnBaseKeyWords.Add("Work Order Number", "9-" + LotNbr);

            return OnBaseKeyWords;
        }
        private List<string> GetMaterialLots(string LotNbr)
        {
            List<string> MatLots = new List<string>();

            JobTreeViewModel JobTreeVM = JobTreeServ.GetJobTreeVM(LotNbr);
            MatLots = JobTreeVM.JobTreeItems.Where(w => w.MaterialLot != null).Select(s => s.MaterialLot).ToList();
            //foreach (JobTreeItem JTItem in JobTreeVM.JobTreeItems)
            //{
            //    if (JTItem.MaterialLot != null)
            //    {
            //        MatLots.Add(JTItem.MaterialLot);
            //    }

            //}
            return MatLots;
        }
        private DocumentQuery GetDocumentQueryObject(string DocType)
        {
            DocumentQuery DocumentQryObj;

            try
            {
                DocumentQryObj = UnityApp.Core.CreateDocumentQuery();
                // Set the Doc type so we only pull the type we want and not other associated types
                DocumentQryObj.AddDocumentType(UnityApp.Core.DocumentTypes.Find(DocType));
            }
            catch (Exception e)
            {
                throw new Exception("Unable to create document query");
            }
            return DocumentQryObj;
        }
        private Application GetUnityAppObject()
        {
            string ServerURL = System.Configuration.ConfigurationManager.AppSettings["OnBaseServerURL"];
            string UserName = System.Configuration.ConfigurationManager.AppSettings["OnBaseUserName"];
            string Password = System.Configuration.ConfigurationManager.AppSettings["OnBasePassword"];
            string DataSource = System.Configuration.ConfigurationManager.AppSettings["OnBaseDataSource"];

            AuthenticationProperties AuthProps = Application.CreateOnBaseAuthenticationProperties(ServerURL, UserName, Password, DataSource);
            AuthProps.LicenseType = LicenseType.Default;
            try
            {
                UnityApp = Hyland.Unity.Application.Connect(AuthProps);
            }
            catch (Exception e)
            {
                LoggingServ.LogError(e);
                throw new Exception("Unable to connect to API service");
            }

            return UnityApp;
        }
    }
}