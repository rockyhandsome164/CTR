using CTR_FLS_2.Models;
using CTR_FLS_2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTR_FLS_2.Services
{
    public class NominalThreadServices : INominalThreadServices
    {
        public void DeleteNominalThread(int Id)
        {
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                NominalThread nominalThread = (from c in context.NominalThreads
                                               where c.Id == Id
                                               select c).FirstOrDefault();
                context.NominalThreads.Remove(nominalThread);
                context.SaveChanges();
            }
        }



        public object GetNominalThreadById(int id)
        {

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                var nominalThread = context.NominalThreads.Where(x => x.Id == id).SingleOrDefault();
                return nominalThread;
            }
        }

        public NominalThread SaveNominalThread(NominalThread nominalThread)
        {
            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                if (nominalThread.Id == 0)
                {
                    context.NominalThreads.Add(nominalThread);
                    context.SaveChanges();
                }
                else
                {
                    NominalThread updatedNominalThread = (from c in context.NominalThreads
                                                          where c.Id == nominalThread.Id
                                                          select c).FirstOrDefault();
                    if (updatedNominalThread.Id > 0)
                    {
                        updatedNominalThread.Id = nominalThread.Id;
                        updatedNominalThread.NominalThreadSize = nominalThread.NominalThreadSize;
                        updatedNominalThread.DashNumber = nominalThread.DashNumber;
                        context.SaveChanges();
                    }
                }
                return nominalThread;
            }
        }

        public List<NominalThreadViewModel> GetAllNominalThread(string searchParams)
        {
            List<NominalThreadViewModel> result = new List<NominalThreadViewModel>();
            IQueryable<NominalThread> dataQuery; 

            using (CTR_FLS_Entities context = new CTR_FLS_Entities())
            {
                dataQuery =  context.NominalThreads.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams))
                {
                    dataQuery = context.NominalThreads.Where(x => x.DashNumber.Contains(searchParams) || x.NominalThreadSize.Contains(searchParams)).AsQueryable();
                }
                return result = dataQuery
                    .Select(x => new NominalThreadViewModel()
                    {
                        Id = x.Id,
                        DashNumber = x.DashNumber,
                        NominalThreadSize = x.NominalThreadSize,

                    }).ToList();
            }
        }

    }
}
