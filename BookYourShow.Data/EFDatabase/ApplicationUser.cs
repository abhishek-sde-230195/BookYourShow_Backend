using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookYourShow.Data.EFDatabase
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "VARCHAR(150)")]
        public string FullName{get;set;}
    }
}