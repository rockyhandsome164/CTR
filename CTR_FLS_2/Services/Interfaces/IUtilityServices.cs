using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CTR_FLS_2.Services
{
    public interface IUtilityServices
    {
        List<MasterSpecViewModel> GetMasterSpecs(MasterSpecSearch masterSpec);
        List<SelectListItem> GetTypes();
        void SaveMasterSpecs(MasterSpec masterSpec);
        void DeleteMasterSpec(int Id);
        void LogError(Exception TheBadStuff);
    }
}
