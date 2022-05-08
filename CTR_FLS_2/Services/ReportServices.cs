using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace CTR_FLS_2.Services
{
    public class ReportServices : IReportServices
    {
        private IJobTreeServices JobTreeServ;
        private ILoggingServices LoggingServ;

        public ReportServices() : this(new JobTreeServices(),
                                       new LoggingServices())
        { }

        public ReportServices(IJobTreeServices JobTreeServicesObj,
                              ILoggingServices LoggingServicesObj)
        {
            JobTreeServ = JobTreeServicesObj;
            LoggingServ = LoggingServicesObj;
        }

        /// <summary>
        /// This method will create a unique file for the CTR report and return the file
        /// name back to the called so that it can be set as the report source for the
        /// report viewer. 
        /// This is needed because we need to dynamically create sub report blocks in the CTR
        /// report based on the number of components this lot has.
        /// There is a base CTR report that will be used as a template. Then XML blocks of sub
        /// report data will be added to the newly create CTR report file. Then the name of the
        /// new file will be returned.
        /// </summary>
        /// <param name="LotNbr">The top level lot number used to generate the CTR report off of</param>
        /// <returns></returns>
        public string GetCTRReportFileName(string LotNbr)
        {
            string NewCTRFileName = "";
            string FullFileName = "";
            string CTRReportPath = HttpContext.Current.Server.MapPath("~/Reports/ReportObjectsInTRDXFormat/");
            string CTRReportTemplate = Path.Combine(CTRReportPath, "CTRContainer.trdx");

            try
            {

                XDocument CTRTRDXFile = XDocument.Load(CTRReportTemplate);
                XElement RootElem = CTRTRDXFile.Root;
                List<XElement> DescendantElements = RootElem.Descendants().ToList();

                bool DetailSectionFound = false;
                XElement ElementToInsertAfter = null;
                XElement DetailsSectionElement = null;
                XElement ContainerMaterialSubReportParameterElement = null;
                XElement ReportLotNbrParameterElement = null;

                foreach (XElement Elem in DescendantElements)
                {
                    if (Elem.Name.LocalName.Equals("DetailSection"))
                    {
                        if (DetailsSectionElement == null)
                        {
                            DetailSectionFound = true;
                            DetailsSectionElement = Elem;
                        }
                    }
                    else
                    {
                        if (Elem.Name.LocalName.Equals("Items"))
                        {
                            if (DetailSectionFound)
                            {
                                if (ElementToInsertAfter == null)
                                {
                                    ElementToInsertAfter = Elem;
                                }
                            }
                        }
                        else
                        {
                            if (Elem.Name.LocalName.Equals("Parameter"))
                            {
                                if (DetailSectionFound)
                                {
                                    List<XAttribute> ParmAttrList = Elem.Attributes().ToList();
                                    foreach(XAttribute Attr in ParmAttrList)
                                    {
                                        if (Attr.Value == "MaterialLotList")
                                        {
                                            List<XElement> ParmDescList = Elem.Descendants().ToList();
                                            foreach(XElement ParmDesc in ParmDescList)
                                            {
                                                if (ParmDesc.Name.LocalName == "String")
                                                {
                                                    ContainerMaterialSubReportParameterElement = ParmDesc;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Elem.Name.LocalName.Equals("ReportParameter"))
                                {
                                    List<XAttribute> ParmAttrList = Elem.Attributes().ToList();
                                    foreach (XAttribute Attr in ParmAttrList)
                                    {
                                        if (Attr.Value == "LotNbr")
                                        {
                                            List<XElement> ParmDescList = Elem.Descendants().ToList();
                                            foreach (XElement ParmDesc in ParmDescList)
                                            {
                                                if (ParmDesc.Name.LocalName == "String")
                                                {
                                                    ReportLotNbrParameterElement = ParmDesc;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Get all the components that we need to create dynamic sub reports for
                JobTreeViewModel JobTreeVM = JobTreeServ.GetJobTreeVM(LotNbr);

                // Create a list of components and their materials that we will add to as we go
                List<ComponentMaterialSpecs> CompMats = new List<ComponentMaterialSpecs>();

                // We also need to generate a list of material specs that are displayed at each component level
                // The rule is that all components will display ALL the material specs for ALL sub components
                // Basically the any parent component will aggregate ALL the materials for their children, grandchildren, etc. 
                // We will traverse the JobTreeVM and pull all materials that are at a level > than the current level
                // of that component. Note that there can be many "branches" so that there could be components
                // at the same level number that have their own set of descendants. Fun, fun fun.
                // This list will be used as a report parameter so the proper material specs are generated for that 
                // component in the report
                if (JobTreeVM.JobTreeItems.Count > 0)
                {
                    // Start the first item level
                    int CurrentWorkingLevel = 0;
                    int ParentPositionInTheList = 0;

                    foreach (JobTreeItem TreeItem in JobTreeVM.JobTreeItems)
                    {
                        if (TreeItem.ComponentLot != null)
                        {
                            // Add to our component list
                            CompMats.Add(new ComponentMaterialSpecs { Component = TreeItem.ComponentLot, ComponentMaterials = new List<string>() });
                            // Set the level in the tree (this is NOT the position in the list)
                            CurrentWorkingLevel = TreeItem.ItemLevel;
                            // Now loop thru ALL child records. Start with the next one in the list (not the one we are on in the foreach)
                            // Break when the level number is >= the current level. Because this denotes
                            // a new branch
                            for (int x = ParentPositionInTheList + 1; x < JobTreeVM.JobTreeItems.Count; x++)
                            {
                                // Get the tree item
                                JobTreeItem WorkTreeItem = JobTreeVM.JobTreeItems[x];
                                if (CurrentWorkingLevel < WorkTreeItem.ItemLevel)
                                {
                                    // See if it's a material lot (don't care about components). 
                                    // The tree item will be one or the other
                                    if (WorkTreeItem.MaterialLot != null)
                                    {
                                        // Have a material lot so add it to the record we just added at the top
                                        // Note that we are associating this to the very top most component in this
                                        // branch which may NOT BE THE DIRECT PARENT. 
                                        CompMats[CompMats.Count - 1].ComponentMaterials.Add(WorkTreeItem.MaterialLot);
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Could be that there are no component lots and only material lots.
                            // In those cases just add a new ComponentMaterialSpec with a blank component but 
                            // the material lots
                            CompMats.Add(new ComponentMaterialSpecs { Component = "", ComponentMaterials = new List<string>() });
                            CompMats[CompMats.Count - 1].ComponentMaterials.Add(TreeItem.MaterialLot);
                        }
                        ParentPositionInTheList++;
                    }
                }

                // Now just get the list of component numbers out of the tree so we can create a specific component container sub report
                List<string> ComponentLots = JobTreeVM.JobTreeItems.Where(w => w.ComponentLot != null).Select(s => s.ComponentLot).ToList();
                int ComponentIterator = 0;

                // Need to grab the namespace of the xml file and use it when creating any XElement. If you don't do this, the XElement will
                // be created an empty xmlns attribute (xmlns="") which causes the report to throw an error at render time. When this value
                // is set, because it matches the parent element (all the way up the tree), then it doesn't put a specific xmlns attribute on 
                // the element. So it's the way to eliminate that attribute on the XElements we are creating.
                XNamespace ReportXMLNS = XNamespace.Get("http://schemas.telerik.com/reporting/2021/2.0");

                // Grab the initial height of the detail section (where the new sub reports will be added) so that we can adjust
                // for every new sub report injected
                double DetailsSectionHeight = Double.Parse(DetailsSectionElement.Attribute("Height").Value.Replace("in", ""));

                // Set a constant for the sub report height
                const double DefaultSubReportHeight = .3;

                // Use this to keep track of the "top" value (Y position) of the sub report needs to be set at. The top value
                // will always be incremented by the DefaultSubReportHeight to determine the next Y position
                double SubReportYPosition = 2.0;

                foreach (string Component in ComponentLots)
                {
                    ComponentIterator++;
                    DetailsSectionHeight += DefaultSubReportHeight;
                    SubReportYPosition += DefaultSubReportHeight;

                    // Extract the list of materials for each component. Format the list into a JSON string that
                    // will be parsed by the stored proc to return data to the report
                    ComponentMaterialSpecs CompMatSpecs = CompMats.FirstOrDefault(f => f.Component == Component);
                    int MatLotIterator = 0;

                    string MatLotList = "[";  // open bracket to start the list
                    if (CompMatSpecs != null)
                    {
                        if (CompMatSpecs.ComponentMaterials.Count > 0)
                        {
                            // List out the material lots enclosed in double quotes
                            foreach(string MatLot in CompMatSpecs.ComponentMaterials)
                            {
                                // The format of the list will be in JSON. The SQL proc
                                // will parse that into a table in order to do the query
                                // We need to preserve the display order (which matches 
                                // how we are extracting the data from list) so it
                                // appears on the form correctly
                                MatLotIterator++;
                                MatLotList += "{\"Ord\":\"" + MatLotIterator + "\", \"Mat\":\"" + MatLot + "\"}, ";
                            }
                            // Remove the list comma and space
                            MatLotList = MatLotList.Substring(0, (MatLotList.Length - 2));
                        }

                        // Add the ending bracket
                        MatLotList += "]";
                    }


                    XElement SubRpt = new XElement(ReportXMLNS  + "SubReport",
                        new XAttribute("Width", "7.500in"),
                        new XAttribute("Height", DefaultSubReportHeight.ToString("#.##") + "in"),
                        new XAttribute("Left", "0in"),
                        new XAttribute("KeepTogether", "False"),
                        new XAttribute("Top", SubReportYPosition.ToString("###.##") + "in"),
                        new XAttribute("Name", "Component" + ComponentIterator.ToString() + "SubReport"),


                            new XElement(ReportXMLNS + "Style",
                                new XElement(ReportXMLNS + "BorderStyle",
                                    new XAttribute("Default", "Solid")
                                )
                            ),

                            new XElement(ReportXMLNS + "ReportSource",
                                new XElement(ReportXMLNS + "UriReportSource",
                                    new XAttribute("Uri", "CTRComponentSubReport.trdx"),
                                    new XElement(ReportXMLNS + "Parameters",
                                        new XElement(ReportXMLNS + "Parameter",
                                            new XAttribute("Name", "MaterialLotList"),
                                            new XElement(ReportXMLNS + "Value",
                                                new XElement(ReportXMLNS + "String", MatLotList)
                                            )
                                        ),
                                        new XElement(ReportXMLNS + "Parameter",
                                            new XAttribute("Name", "ConnectionString"),
                                            new XElement(ReportXMLNS + "Value",
                                                new XElement(ReportXMLNS + "String", "= Parameters.ConnectionString.Value")
                                            )
                                        ),
                                        new XElement(ReportXMLNS + "Parameter",
                                            new XAttribute("Name", "ComponentNbr"),
                                            new XElement(ReportXMLNS + "Value",
                                                new XElement(ReportXMLNS + "String", ComponentIterator.ToString())
                                            )
                                        ),
                                        new XElement(ReportXMLNS + "Parameter",
                                            new XAttribute("Name", "LotNbr"),
                                            new XElement(ReportXMLNS + "Value",
                                                new XElement(ReportXMLNS + "String", Component)
                                            )
                                        )
                                    )
                                )
                            )

                    ) ;
                    ElementToInsertAfter.Add(SubRpt);
                }

                // Adjust the height of the detail section based on the number of components we have
                DetailsSectionElement.Attribute("Height").Value = DetailsSectionHeight.ToString("###.##");

                // Need to set the "unique" list of material lot numbers at the top container level
                // At the component level, the list is for that component and it's children
                // For the container, it needs a unique list of EVERY material lot for all components
                string UniqueMatLots = "[";
                int UniqueMatLotIterator = 0;

                foreach (ComponentMaterialSpecs AllCompMatSpecs in CompMats)
                {
                    foreach(string AllCompMatSpec in AllCompMatSpecs.ComponentMaterials)
                    {
                        if (UniqueMatLots.IndexOf("\"" + AllCompMatSpec + "\"") == -1)
                        {
                            // This material not in the list yet
                            // The format of the list will be in JSON. The SQL proc
                            // will parse that into a table in order to do the query
                            // We need to preserve the display order (which matches 
                            // how we are extracting the data from list) so it
                            // appears on the form correctly
                            UniqueMatLotIterator++;
                            UniqueMatLots += "{\"Ord\":\"" + UniqueMatLotIterator + "\", \"Mat\":\"" + AllCompMatSpec + "\"}, ";
                            //UniqueMatLots += "\"" + AllCompMatSpec + "\", ";
                        }
                    }
                }
                // Remove the list comma and space
                UniqueMatLots = UniqueMatLots.Substring(0, (UniqueMatLots.Length - 2));
                // Add the ending bracket
                UniqueMatLots += "]";

                if (ContainerMaterialSubReportParameterElement != null)
                {
                    ContainerMaterialSubReportParameterElement.Value = UniqueMatLots;
                }

                if (ReportLotNbrParameterElement != null)
                {
                    ReportLotNbrParameterElement.Value = LotNbr;
                }

                // Now create a unique file name
                string FileNameSuffix = DateTime.Now.ToString("yyyyMMddHHmmss");
                FullFileName = LotNbr + "_" + FileNameSuffix + ".trdx";

                NewCTRFileName = CTRReportPath + FullFileName;

                // Save the file
                CTRTRDXFile.Save(NewCTRFileName);
            }
            catch (Exception Ex)
            {
                // For testing. Shoot the exception back up to the UI so we can display the message on the console
                NewCTRFileName = Ex.StackTrace;
                LoggingServ.LogError(Ex);
            }

            // Return just the file name
            return FullFileName;
        }

        public string DetermineLockingTorqueRptNeeded(string LotNbr)
        {
            string RptNeeded = "NO";
            int DashPos = LotNbr.IndexOf("-");
            string JobPortion = LotNbr.Substring(0, DashPos);
            int? SuffixPortion = Int32.Parse(LotNbr.Substring(DashPos, LotNbr.Length - DashPos - 1));
            List<int?> TemplateLists = new List<int?>() { 9, 10, 11 };
            List<int?> TemplatesForJob = new List<int?>();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    TemplatesForJob = DBContext.Tests.Where(w => w.Job == JobPortion && w.Suffix == SuffixPortion).Select(s => s.Template).ToList();

                    if (TemplatesForJob.Count > 0)
                    {
                        if (TemplatesForJob.Any(a => TemplateLists.Contains(a)))
                        {
                            RptNeeded = "YES";
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                LoggingServ.LogError(Ex);
            }

            return RptNeeded;

        }
        public string ValidateJobItemRelationship(string LotNbr, string Item)
        {
            string ItemJobRelated = "NO";
            int DashPos = LotNbr.IndexOf("-");
            string JobPortion = LotNbr.Substring(0, DashPos);
            int? SuffixPortion = Int32.Parse(LotNbr.Substring(DashPos, LotNbr.Length - DashPos - 1));

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    if (DBContext.JobInfoes.Where(w => w.Job == JobPortion && w.Suffix == SuffixPortion && w.Item == Item).Count() > 0)
                    {
                        ItemJobRelated = "YES";
                    }
                }
            }
            catch (Exception Ex)
            {
                LoggingServ.LogError(Ex);
            }

            return ItemJobRelated;

        }
    }
}