using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // הוסיפי את זה למעלה

namespace TodoApi;

public partial class Item
{
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("isComplete")]
    public bool? IsComplete { get; set; }
}
