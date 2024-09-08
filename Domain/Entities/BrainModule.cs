using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BrainModule
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int GroupId { get; set; }

        [Column]
        public int BranchId { get; set; }

        [Column]
        public string TextField { get; set; }

        [Column]
        public string Ip { get; set; }

    }
}
