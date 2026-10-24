namespace WebApp.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Contact
    {
        [Key]
        public long Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [RegularExpression(@"^0[0-9]+"), StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }
        [StringLength(200)]
        public string? Instagram { get; set; }

    }
}
