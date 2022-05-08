using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTR_FLS_2.Models
{
    /// <summary>
    /// Class used to manage the relationship between components and their associated materials
    /// When printing a CTR, each component needs to display ALL the material specs for ALL 
    /// sub components.
    /// </summary>
    public class ComponentMaterialSpecs
    {
        public string Component { get; set; }
        public List<string> ComponentMaterials { get; set; }
    }
}
