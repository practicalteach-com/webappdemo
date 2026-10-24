using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Model;

namespace WebApp.Pages.ContactPages;

public class DeleteModel : PageModel
{
    private readonly ContactDbContext _context;

    public DeleteModel(ContactDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Contact Contact { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var contact = await _context.Contact.FirstOrDefaultAsync(m => m.Id == id);
        if (contact is null)
        {
            return NotFound();
        }
        else
        {
            Contact = contact;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var contact = await _context.Contact.FindAsync(id);
        if (contact != null)
        {
            Contact = contact;
            _context.Contact.Remove(Contact);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
