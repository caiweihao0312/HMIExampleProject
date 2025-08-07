using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sale.Domain
{
    [Table("T_Product")] // 映射表名
    public class Product
    {
        public int Id { get; set; }

        [Column("ProductName")] // 映射字段名
        public string Name { get; set; }

        [Column("ProductCount")]
        public int Count { get; set; }
    }
}
