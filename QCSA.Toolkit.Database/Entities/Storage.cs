using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QCSA.Toolkit.Database.Entities
{
    [Table("Storage")]
    class Storage
    {
        [Key]
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Path { get; set; }
        public string Datetime { get; set; }


        [ForeignKey("User_Id")]
        public virtual Users User { get; set; }
    }

}
  
    