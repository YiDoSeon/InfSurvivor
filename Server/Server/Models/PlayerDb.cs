using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
    }
}