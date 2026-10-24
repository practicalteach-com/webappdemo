namespace WebApp.Model
{
    using System.ComponentModel.DataAnnotations;

    public class Contact
    {
        [Key]
        public long Id { get; set; }
        public string? Name { get; set; }
        [RegularExpression(@"^0[0-9]+")]
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Instagram { get; set; }

    }
}
