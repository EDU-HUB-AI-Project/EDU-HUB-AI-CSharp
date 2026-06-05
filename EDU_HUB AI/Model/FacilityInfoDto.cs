using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EDU_HUB_AI.Model
{
    public class FacilityInfoDto
    {
        public string facilityId { get; set; }
        public string facilityType { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public int? floor { get; set; }
        public decimal? mapX { get; set; }
        public decimal? mapY { get; set; }
        public string imagePath { get; set; }
        public string description { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string delYn { get; set; }

        public override string ToString()
        {
            return $"facilityId: {facilityId}, facilityType: {facilityType}, name: {name}, " +
                   $"location: {location}, floor: {floor}, mapX: {mapX}, mapY: {mapY}, " +
                   $"imagePath: {imagePath}, description: {description}, " +
                   $"createdAt: {createdAt}, updatedAt: {updatedAt}, delYn: {delYn}";
        }
    }
}
