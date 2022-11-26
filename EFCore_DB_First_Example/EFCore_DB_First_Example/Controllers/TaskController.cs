using EFCore_DB_First_Example.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore_DB_First_Example.Controllers;

public class TaskController : Controller
{
    private readonly MireroContext _context;

    public TaskController(MireroContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Tasks.ToListAsync());
    }
}
