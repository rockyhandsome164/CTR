using CTR_FLS_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTR_FLS_2.Services
{
    public interface ITestTemplateService
    {
        List<TestTemplateType> GetTestTemplateTypes();
        void SaveTestTemplate(TestTemplate testTemplate);
        List<TestTemplate> GetTestTemplateDetails();
        void DeleteTestTemplate(int id);
    }
}
