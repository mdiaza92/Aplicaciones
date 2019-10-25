using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static WS_HyJ.Models.Enum.Enums;

namespace WS_HyJ.Models.Requests
{
    public class KardexRequest
    {
        [Required]
        public string IdProducto { get; set; }
                
        public short? MinimumAmount { get; set; }

        public short? CurrentAmount { get; set; }
    }
}
