using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class CafeteriaDto
    {
        public string cafeteriaId { get; set; }
        public string mealDate { get; set; }
        public string mealType { get; set; }
        public string menu { get; set; }
        public string mealClosed { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string delYn { get; set; }
    }
}
