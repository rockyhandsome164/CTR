using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.Services
{
    public interface IItemSpecServices
    {
        List<ItemSpec> GetAllSpecification( );
        List<ItemViewModel> GetAllItems(string searchParams);
        List<MasterSpecViewModel> GetMasterSpecs();
        Item SaveItems(Item item);
        ItemSpec SaveItemSpec(ItemSpec itemSpec);


    }
}