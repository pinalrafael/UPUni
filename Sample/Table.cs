using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    public class Table
    {
        [UPUni.Attributes.Table(true, "ID")]
        public int Id { get; set; }
        [UPUni.Attributes.Table(true, "NAME")]
        public string Name { get; set; }
        [UPUni.Attributes.Table(true, "DESCRIPTION")]
        public string Description { get; set; }
        [UPUni.Attributes.Table(true, "PRICE", "$ {0:0,0.00}")]
        public double Price { get; set; }
        [UPUni.Attributes.Table(true, "STATE")]
        public int State { get; set; }
        [UPUni.Attributes.Table(false, "TYPE")]
        public string Type { get; set; }
    }
}
