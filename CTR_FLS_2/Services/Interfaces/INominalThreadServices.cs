using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System.Collections.Generic;

namespace CTR_FLS_2.Services
{
    public interface INominalThreadServices
    {

        List<NominalThreadViewModel> GetAllNominalThread(string searchParams);
        void DeleteNominalThread(int Id);
        NominalThread SaveNominalThread(NominalThread nominalThread);
        object GetNominalThreadById(int id);
    }
}
