using System.ComponentModel.DataAnnotations.Schema;

namespace EucharisticMiracles.Models
{
    public class Users
    {
        public string Username { get; set; }

        [ForeignKey("password")]
        public string Password { get; set; }
    }
}
