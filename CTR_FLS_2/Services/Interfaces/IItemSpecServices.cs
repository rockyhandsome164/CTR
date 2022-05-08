using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System.Collections.Generic;

namespace CTR_FLS_2.Services
{
    public interface IItemSpecServices
    {
        List<ItemViewModel> GetAllItems(string searchParams);
        Item SaveItems(Item item);
        void AddMasterSpecToItem(ItemSpecsViewModel itemSpecsViewModel);
        List<ItemSpec> GetItemSpecsByItemNumber(string itemNumber);
        List<Note> GetNoteByItemNumber(string itemNumber);
        NoteViewModel SaveItemNote(NoteViewModel note);
        List<ProductFamilyViewModel> GetAllProductFamily(string searchParams);
        List<TestTemplate> GetTestDetail(string searchParams);
        void SaveTest(Test test);
    }
}