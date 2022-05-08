using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTR_FLS_2.Services
{
    public static class Constants
    {   

        #region OnBase Constants
        public const string ONBASE_NO_DOCS_FOUND_MSG = "No documents found";

        public const string ONBASE_KEYWORD_TYPE_LOT_NBR = "Lot Number";
        public const string ONBASE_KEYWORD_TYPE_MATERIAL_LOT_NBR = "Material Lot Number";

        public const string ONBASE_DOC_OUTPUT_TYPE_FILE = "File";
        public const string ONBASE_DOC_OUTPUT_TYPE_STREAM = "Stream";
        public const string ONBASE_DOC_OUTPUT_TYPE_EXISTANCE = "Exists";

        public const string ONBASE_DOC_TYPE_TEST_REPORT = "MFG QA - Test Report";
        public const string ONBASE_DOC_TYPE_MATERIAL_CERT = "MFG QA - Material Cert";
        public const string ONBASE_DOC_TYPE_OSP_CERT = "MFG QA - Outside Processing Cert";
        public const string ONBASE_DOC_TYPE_CERT_PKG = "MFG QA - Cert Package";

        #endregion
    }
}