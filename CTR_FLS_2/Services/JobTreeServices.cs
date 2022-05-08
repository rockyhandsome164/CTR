using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.Services
{
    public class JobTreeServices : IJobTreeServices
    {
        private readonly ICommonServices CommonServ;
        private readonly IQADServices QADServ;

        private bool QADLot = false;

        // Class var's for tree view display
        private List<int> ParentDriverItems = new List<int>();
        private List<int> ChildDriverItems = new List<int>();
        private List<JobTreeViewIterationResults> IntermediaryResultItems = new List<JobTreeViewIterationResults>();
        private List<JobTreeItem> JobTreeItems = new List<JobTreeItem>();
        private List<JobTreeMaterialViewModel> JobTreeMaterials = new List<JobTreeMaterialViewModel>();

        public JobTreeServices() : this(new CommonServices(),
                                        new QADServices()) {  }

        public JobTreeServices(ICommonServices CommonServicesObj,
                               IQADServices QADServicesObj)
        {
            CommonServ = CommonServicesObj;
            QADServ = QADServicesObj;
        }

        #region BuildTree
        public JobTreeViewModel GetJobTreeVM(string TopLevelComponent)
        {
            JobTreeViewModel VM = new JobTreeViewModel();
            VM.Message = "OK";

            bool HaveAllChildren = false;
            int LevelNbr = 0;

            // Init
            ParentDriverItems.Clear();
            ChildDriverItems.Clear();
            IntermediaryResultItems.Clear();

            // See if this is a legacy (syteline)lot or a lot in QAD
            QADLot = QADServ.IsQADLotNbr(TopLevelComponent);

            // Set Parent
            int TopLevelId = CommonServ.GetComponentItemId(TopLevelComponent);

            if (TopLevelId > 0)
            {

                // NOTE: Some top level items are added manually in the database now as they are not part 
                // of the import process. So a dummy itemcode is set. Not suitable for display.
                // Depending on where the user begins their search, we may have it. Check for the dummy
                // code and if present just set to blank.
                // Also get the item code for the parent to send back to the UI for display
                JobLot TopLevelItem = CommonServ.GetComponentItem(TopLevelId);
                VM.TopLevelComponentItemCode = TopLevelItem.ItemCode.Equals("TOP") ? "" : TopLevelItem.ItemCode;

                Item TopLevelItemData = CommonServ.GetItem(VM.TopLevelComponentItemCode);
                VM.TopLevelComponentItemDesc = TopLevelItemData.Description;

                // Get the cert info for the top level component
                Cert CertInfo = CommonServ.GetCertForLot(TopLevelItem.ComponentLot);
            
                if (CertInfo != null)
                {
                    VM.CertNumber = CertInfo.CertNbr;
                    VM.CertDate = CertInfo.CertDate.Value;
                }

                // Get OSP data for top level component
                List<OSP> OspRecs = CommonServ.GetOSP(TopLevelItem.ComponentLot);
                // Since there could be multiple OSPs, concatentate into a string for display purposes
                string OSPList = "";
                foreach (OSP OSPRec in OspRecs)
                {
                    OSPList += OSPRec.AttachmentType + ", ";
                }
                if (OSPList.Length > 0)
                {
                    // Strip off last ", "
                    OSPList = OSPList.Substring(0, OSPList.Length - 2);
                }

                VM.TopLevelComponentOSP = OSPList;

                // Add our parent item to the top level driver IDs
                ParentDriverItems.Add(TopLevelId);

                while (!HaveAllChildren)
                {
                    // Bump up the level nbr
                    LevelNbr++;

                    // kill any existing children (in the "coding" sense  :-)  )
                    ChildDriverItems.Clear();

                    // Go thru all the children from this parent and 
                    // get their results
                    foreach (int Child in ParentDriverItems)
                    {
                        // loop thru all the children to get the grandkids
                        LoadIntermediaryResults(LevelNbr, Child);
                    }

                    // If we have more children, make them parents and re-do 
                    // the process
                    if (ChildDriverItems.Count > 0)
                    {
                        ParentDriverItems.Clear();
                        ParentDriverItems = ChildDriverItems.ToList();
                    }
                    else
                    {
                        HaveAllChildren = true;
                    }
                }

                LoadTreeItemsForParent();

                VM.JobTreeItems = JobTreeItems;
                VM.MaterialList = JobTreeMaterials;
            }
            else
            {
                VM.Message = "Lot Number is not found";
            }

            return VM;
        }

        private void LoadIntermediaryResults(int Level, int ParentId)
        {
            List<int> GrandKids = CommonServ.GetItemChildren(ParentId);
            foreach (int Kid in GrandKids)
            {
                JobLot ParentItem = CommonServ.GetComponentItem(ParentId);
                JobLot KidItem = CommonServ.GetComponentItem(Kid);

                IntermediaryResultItems.Add(new JobTreeViewIterationResults
                {
                    Level = Level,
                    ChildItemId = Kid,
                    ParentItemId = ParentItem.Id,
                    Sequence = KidItem.Sequence.Value
                });

                if (!String.IsNullOrEmpty(KidItem.ComponentLot))
                {
                    ChildDriverItems.Add(Kid);
                }
            }
        }

        private void LoadTreeItemsForParent()
        {
            List<JobTreeViewIterationResults> TopLevelItems = IntermediaryResultItems.Where(w => w.Level == 1).OrderBy(o => o.Sequence).ToList();
            foreach (JobTreeViewIterationResults TopLevelItem in TopLevelItems)
            {
                LoadTreeItem(TopLevelItem);

                LoadTreeItemsForChildren(TopLevelItem.ChildItemId);
            }
        }

        private void LoadTreeItemsForChildren(int ParentItemId)
        {
            List<JobTreeViewIterationResults> ChildItems = IntermediaryResultItems.Where(w => w.ParentItemId == ParentItemId).OrderBy(o => o.Sequence).ToList();
            foreach (JobTreeViewIterationResults ChildItem in ChildItems)
            {
                LoadTreeItem(ChildItem);

                LoadTreeItemsForChildren(ChildItem.ChildItemId);
            }

        }

        private void LoadTreeItem(JobTreeViewIterationResults JobTreeResult)
        {

            JobTreeItem TreeItem = new JobTreeItem();
            JobLot ComponentItem = CommonServ.GetComponentItem(JobTreeResult.ChildItemId);

            // Get any Material data
            LotInfo LotInfoRec = new LotInfo();

            if (!String.IsNullOrWhiteSpace(ComponentItem.MaterialLot))
            {

                // Note that LotInfo will contain both Legacy (Sytline) and QAD lot data
                LotInfoRec = CommonServ.GetLotInfo(ComponentItem.MaterialLot);
                if (LotInfoRec != null)
                {
                    // Get vendor info
                    string VendorName = "";
                    if (LotInfoRec.VendorId != null)
                    {
                        // Check if we need to get the data from legacy tables or QAD
                        // This is set up top as soon as we come in 
                        if (QADLot)
                        {
                            VendorName = QADServ.GetVendorName(LotInfoRec.VendorId);
                        }
                        else
                        {
                            Vendor VendorInfo = CommonServ.GetVendorInfo(LotInfoRec.VendorId);
                            if (VendorInfo != null)
                            {
                                VendorName = VendorInfo.Name;
                            }
                        }
                    }

                    // Get Item data
                    string ItemDesc = "";
                    // Check if we need to get the data from legacy tables or QAD
                    // This is set up top as soon as we come in 
                    if (QADLot)
                    {
                        ItemDesc = QADServ.GetPartDesc(ComponentItem.ItemCode);
                    }
                    else
                    {
                        Item ItemData = CommonServ.GetItem(ComponentItem.ItemCode);
                        if (ItemData != null)
                        {
                            ItemDesc = ItemData.Description;
                        }
                    }

                    JobTreeMaterialViewModel MatVM = new JobTreeMaterialViewModel
                    {
                        Id = LotInfoRec.Id,
                        ItemCode = ComponentItem.ItemCode,
                        ControlNbr = ComponentItem.MaterialLot,
                        VendorNbr = LotInfoRec.VendorId,
                        MillNbr = LotInfoRec.MillNbr,
                        MaterialSpec = LotInfoRec.MaterialSpec,
                        Stage = "",
                        VendorName = VendorName,
                        Description = ItemDesc,
                        Ponbr = LotInfoRec.RefNbr,
                        RecvdDate = LotInfoRec.DateCreated,
                        POLineNbr = LotInfoRec.RefSuffix,
                        POReleaseNbr = LotInfoRec.RefRelease
                    };

                    JobTreeMaterials.Add(MatVM);
                }

            }

            // Get any OSP data
            List<OSP> OspRecs = CommonServ.GetOSP(ComponentItem.ComponentLot);
            // Since there could be multiple OSPs for one component, concatentate into a string 
            // for display purposes
            string OSPList = "";
            foreach(OSP OSPRec in OspRecs)
            {
                OSPList += OSPRec.AttachmentType + ", ";
            }
            if (OSPList.Length > 0)
            {
                // Strip off last ", "
                OSPList = OSPList.Substring(0, OSPList.Length - 2);
            }

            TreeItem.ItemCode = ComponentItem.ItemCode;
            TreeItem.ComponentLot = ComponentItem.ComponentLot;
            TreeItem.ItemLevel = JobTreeResult.Level;
            TreeItem.MaterialLot = ComponentItem.MaterialLot;
            TreeItem.VendorNbr = LotInfoRec != null ? LotInfoRec.VendorId : "";
            TreeItem.MaterialSpec = LotInfoRec != null ? LotInfoRec.MaterialSpec : "";
            TreeItem.VendorMillNbr = LotInfoRec != null ? LotInfoRec.MillNbr : "";
            TreeItem.OSPAttachType = OSPList;

            JobTreeItems.Add(TreeItem);
        }
        #endregion
    }
}
