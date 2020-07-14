using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.DBTables
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "VARCHAR(150)")]
        public string FirstName { get; set; }
        [Column(TypeName = "VARCHAR(150)")]
        public string LastName { get; set; }
    }
}
