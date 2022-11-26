using System;
using System.Collections.Generic;

namespace EFCore_DB_First_Example.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public DateTime CreatedOn { get; set; }
}
