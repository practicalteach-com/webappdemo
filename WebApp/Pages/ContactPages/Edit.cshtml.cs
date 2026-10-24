using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Model;

namespace WebApp.Pages.ContactPages;

public class EditModel : PageModel
{
    private readonly ContactDbContext _context;

    public EditModel(ContactDbContext context)
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
        Contact = contact;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Contact).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ContactExists(Contact.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool ContactExists(long id)
    {
        return _context.Contact.Any(e => e.Id == id);
    }
}
