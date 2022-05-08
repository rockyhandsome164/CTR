using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.Services
{
    public class TestTemplateService : ITestTemplateService
    {
        private readonly IUtilityServices UtilServ;
        public TestTemplateService() : this(new UtilityServices()) { }
        public TestTemplateService(IUtilityServices UtilityServicesObj)
        {
            UtilServ = UtilityServicesObj;
        }

        public List<TestTemplateType> GetTestTemplateTypes()
        {
            List<TestTemplateType> testTemplateTypes = new List<TestTemplateType>();

            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    testTemplateTypes = DBContext.Tests.Select(s => new TestTemplateType { Template = s.Template, Name = s.Name }).Distinct().ToList();
                }
            }
            catch (Exception Ex)
            {
                UtilServ.LogError(Ex);
            }

            return testTemplateTypes;
        }

        public void SaveTestTemplate(TestTemplate testTemplate)
        {
            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    if (testTemplate != null)
                    {
                        DBContext.TestTemplate.Add(testTemplate);
                        DBContext.SaveChanges();
                    }
                }
            }
            catch (Exception Ex)
            {
                UtilServ.LogError(Ex);
            }
        }

        public List<TestTemplate> GetTestTemplateDetails()
        {
            List<TestTemplate> testTemplates = new List<TestTemplate>();
            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    testTemplates= DBContext.TestTemplate.ToList();
                }
            }
            catch (Exception Ex)
            {
                UtilServ.LogError(Ex);
            }
            return testTemplates;
        }

        public void DeleteTestTemplate(int id)
        {
            try
            {
                using (CTR_FLS_Entities DBContext = new CTR_FLS_Entities())
                {
                    TestTemplate testTemplate = DBContext.TestTemplate.Where(x => x.Test == id).FirstOrDefault();
                    DBContext.TestTemplate.Remove(testTemplate);
                    DBContext.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                UtilServ.LogError(Ex);
            }
        }
    }
}