using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CTR_FLS_2.Services
{
    public class ItemSpecServices : IItemSpecServices
    {
        public List<ItemViewModel> GetAllItems(string searchParams)
        {
            List<ItemViewModel> itemViewModels = new List<ItemViewModel>();
            IQueryable<Item> dataQuery;

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                dataQuery = context.Items.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    dataQuery = context.Items.Where(x => x.Item1.Contains(searchParams) || x.Description.Contains(searchParams)).AsQueryable();
                }
                return itemViewModels = dataQuery
                    .Select(x => new ItemViewModel()
                    {
                        Id = x.Id,
                        Item1 = x.Item1,
                        Description = x.Description,
                        Type = x.Type,
                        NominalThreadSize = x.NominalThreadSize,
                        ProductFamily = x.ProductFamily,
                        Status = x.Status,
                        MaterialStatus = x.MaterialStatus
                    }).ToList();
            }

        }

        public List<ProductFamilyViewModel> GetAllProductFamily(string searchParams)
        {
            List<ProductFamilyViewModel> result = new List<ProductFamilyViewModel>();
            IQueryable<ProductFamily> dataQuery;

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                dataQuery = context.ProductFamily.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    dataQuery = context.ProductFamily.Where(x => x.ProductFamily1.Contains(searchParams) || x.Description.Contains(searchParams)).AsQueryable();
                }
                return result = dataQuery
                   .Select(x => new ProductFamilyViewModel()
                   {
                       Id = x.Id,
                       ProductFamily = x.ProductFamily1,
                       Description = x.Description
                   }).ToList();
            }
        }

        public Item SaveItems(Item item)
        {
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                if (item.Id == 0)
                {
                    context.Items.Add(item);
                    context.SaveChanges();
                }
                else
                {
                    Item UpdatedItem = (from c in context.Items
                                        where c.Id == item.Id
                                        select c).FirstOrDefault();
                    if (UpdatedItem.Id > 0)
                    {
                        UpdatedItem.Id = item.Id;
                        UpdatedItem.Item1 = item.Item1;
                        UpdatedItem.Description = item.Description;
                        UpdatedItem.Type = item.Type;
                        UpdatedItem.NominalThreadSize = item.NominalThreadSize;
                        UpdatedItem.ProductFamily = item.ProductFamily;
                        UpdatedItem.Status = item.Status;
                        UpdatedItem.MaterialStatus = item.MaterialStatus;
                        context.SaveChanges();
                    }
                }
                return item;
            }
        }

        public void AddMasterSpecToItem(ItemSpecsViewModel itemSpecsViewModel)
        {
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {

                ItemSpec itemSpecToInsert = new ItemSpec
                {
                    EnteredBy = "User Name",
                    DateEntered = DateTime.UtcNow,
                    Description = itemSpecsViewModel.Description,
                    Spec = itemSpecsViewModel.Spec,
                    ItemType = itemSpecsViewModel.ItemType,
                    Item = itemSpecsViewModel.ItemNumber,
                    AuditKey = null,

                };
                context.ItemSpecs.Add(itemSpecToInsert);
                context.SaveChanges();
            }

        }

        public List<ItemSpec> GetItemSpecsByItemNumber(string itemNumber)
        {
            List<ItemSpec> itemSpec = null;
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                itemSpec = context.ItemSpecs.Where(x => x.Item == itemNumber).OrderByDescending(x => x.DateEntered).ToList();
            }
            return itemSpec;
        }

        public List<Note> GetNoteByItemNumber(string itemNumber)
        {
            IQueryable<Note> dataQuery;
            List<Note> notes = new List<Note>();
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                dataQuery = (from itemInfo in context.ItemInfoes
                             join noteItem in context.Notes on itemInfo.TextKey equals noteItem.TextKey
                             where itemInfo.Item == itemNumber &&
                             !(noteItem.Notes == null || noteItem.Notes.Equals(string.Empty))
                             select noteItem).OrderByDescending(X => X.Notes).AsQueryable();

                if (dataQuery.Any()) return dataQuery.ToList();

                var note = new Note
                {
                    TextKey = context.ItemInfoes.Where(x => x.Item == itemNumber).Select(x => x.TextKey).SingleOrDefault(),
                    Notes = string.Empty
                };
                notes.Add(note);
            }
            return notes;
        }

        public NoteViewModel SaveItemNote(NoteViewModel note)
        {
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {

                if (note.Id > 0)
                {
                    var existingNote = context.Notes.SingleOrDefault(x => x.Id == note.Id);
                    if (existingNote != null)
                    {
                        existingNote.Notes = note.Notes;
                    }
                }
                else
                {
                    Note newNote = new Note()
                    {
                        Notes = note.Notes,
                        TextKey = note.TextKey,
                        NewParagraph = note.NewParagraph,
                        LineNbr = note.LineNbr,
                    };

                    context.Notes.Add(newNote);

                }

                context.SaveChanges();

            }

            return note;
        }

        public List<TestTemplate> GetTestDetail(string searchParams)
        {
            List<TestTemplate> testTemplates = new List<TestTemplate>();
            IQueryable<TestTemplate> dataQuery;

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                dataQuery = context.TestTemplate.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    dataQuery =
                        context.TestTemplate.Where(x => x.Requirements.Contains(searchParams) || x.Name.Contains(searchParams) || x.Test.ToString().Contains(searchParams))
                        .AsQueryable();

                }
                return testTemplates = dataQuery.ToList();
                //.Select(x => new Test()
                //{
                //    Id = x.Id,

                //    Tes = x.Item1,
                //    Description = x.Description,
                //    Type = x.Type,
                //    NominalThreadSize = x.NominalThreadSize,
                //    ProductFamily = x.ProductFamily,
                //    Status = x.Status,
                //    MaterialStatus = x.MaterialStatus
                //}).ToList();
            }

        }

        public void SaveTest(Test test)
        {
            IQueryable<Test> dataQuery;
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {

                if (test.Id == 0)
                {
                    context.Tests.Add(test);
                    context.SaveChanges();
                }
                else
                {
                    dataQuery = (from c in context.Tests
                                 where c.Id == test.Id
                                 select c).AsQueryable();

                    if (dataQuery.Any())
                    {
                        dataQuery
                      .Select(x => new Test()
                      {
                          Id = x.Id,
                          Item = x.Item,
                          Job = x.Job,
                          Test1 = x.Test1,
                          Name = x.Name,
                          TestBolt = x.TestBolt,
                          DateEntered = DateTime.UtcNow,
                          EnteredBy = "User Name",
                          Frequency = x.Frequency,
                          Requirements = x.Requirements,
                          ResultCycles = x.ResultCycles,
                          SeatLoad = x.SeatLoad,
                          UnitOfMeasure = x.UnitOfMeasure,
                          CyclesSamples = x.CyclesSamples


                      }).ToList();
                    }
                }

            }
        }


    }
}