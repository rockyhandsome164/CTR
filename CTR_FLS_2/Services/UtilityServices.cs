using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CTR_FLS_2.Services
{
    public class UtilityServices : IUtilityServices
    {
        public void DeleteMasterSpec(int Id)
        {
            try
            {
                using (CTR_FLS_Entities context = new CTR_FLS_Entities())
                {
                    MasterSpec masterSpec = context.MasterSpecs.Where(x => x.Id == Id).FirstOrDefault(); //returns a single item.
                    context.MasterSpecs.Remove(masterSpec);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw ;
            }
        }

        public List<MasterSpecViewModel> GetMasterSpecs(MasterSpecSearch masterSpec)
        {
            List<MasterSpecViewModel> result = new List<MasterSpecViewModel>();
            List<MasterSpec> dataQuery = new List<MasterSpec>();

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                if (masterSpec.Specification != null || masterSpec.Master != null || masterSpec.DefaultTypeId > 0)
                {
                    dataQuery = context.MasterSpecs
                                          .Where(x => (masterSpec.Specification == null || x.Specification.Contains(masterSpec.Specification))
                                                    && (masterSpec.Master == null || x.Master.Contains(masterSpec.Master))
                                                        && (masterSpec.DefaultTypeId == 0 || masterSpec.DefaultTypeId == null || (x.DefaultTypeId.ToString().Contains(masterSpec.DefaultTypeId.ToString())))).ToList();

                }
                else
                {
                    dataQuery = context.MasterSpecs.ToList();

                }

                if (dataQuery.Count() > 0 )
                {

                    result = dataQuery
                        .Select(x => new MasterSpecViewModel()
                        {
                            Id = x.Id,
                            Specification = x.Specification,
                            Description = x.Description,
                            Master = x.Master,
                            EnteredBy = x.EnteredBy,
                            DateEntered = x.DateEntered,
                            DefaultTypeId=x.DefaultTypeId,
                            DefaultType = new DefaultType()
                            {
                                DefaultType1 = x.DefaultType?.DefaultType1 ?? null,
                                Id = x.DefaultTypeId 
                               
                            }
                            
                        }).ToList();
                }
                return result;
            }
        }

        public List<SelectListItem> GetTypes()
        {
            List<SelectListItem> defaultTypes = new List<SelectListItem>();
           
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                defaultTypes = context.DefaultTypes.Select(x => new SelectListItem()
                {
                    Text = x.DefaultType1,
                    Value = x.Id.ToString()
                }).OrderBy(x=>x.Text).ToList();
            }
             return defaultTypes;
        }

        public void SaveMasterSpecs(MasterSpec masterSpec)
        {
            //TODO : replace this with the actual user 
          
             masterSpec.EnteredBy = "User Name";
            masterSpec.DateEntered = DateTime.UtcNow;          
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                if (masterSpec.Id == 0)
                {
                    context.MasterSpecs.Add(masterSpec);
                    context.SaveChanges();
                }
                else
                {
                    MasterSpec updatedMasterSpec = (from c in context.MasterSpecs
                                                          where c.Id == masterSpec.Id
                                                          select c).FirstOrDefault();

                    if (updatedMasterSpec.Id > 0)
                    {
                        updatedMasterSpec.Id = masterSpec.Id;
                        updatedMasterSpec.Specification = masterSpec.Specification;
                        updatedMasterSpec.Description = masterSpec.Description;
                        updatedMasterSpec.Master = masterSpec.Master;
                        updatedMasterSpec.DefaultTypeId = masterSpec.DefaultTypeId;
                        context.SaveChanges();
                    }

                }
               
            }   
        }

        public void LogError(Exception TheBadStuff)
        {

        }

    }
}
