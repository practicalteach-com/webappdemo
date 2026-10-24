using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Model;

namespace WebApp.Pages.ContactPages;

public class IndexModel : PageModel
{
    private readonly ContactDbContext _context;

    public IndexModel(ContactDbContext context)
    {
        _context = context;
    }

    public IList<Contact> Contact { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Contact = await _context.Contact.ToListAsync();
    }
}
