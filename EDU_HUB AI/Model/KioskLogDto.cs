using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EDU_HUB_AI.Model
{
    public class KioskLogDto
    {
        public string logId { get; set; }
        public string action { get; set; }
        public string createdAt { get; set; }
        public string delYN { get; set; }

        public override string ToString()
        {
            return $"logId: {logId}, action: {action}, createdAt: {createdAt}, " +
                   $"delYN: {delYN}";
        }
    }
}
