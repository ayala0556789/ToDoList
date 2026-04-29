using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace TodoApi;



public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            // entity.HasKey(e => e.Id).HasName("PRIMARY");

            // entity.ToTable("items");

            // entity.Property(e => e.Name).HasMaxLength(100);

            
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("items"); // שם הטבלה באותיות קטנות

            // הגדרה מפורשת של שמות העמודות כפי שהם ב-Workbench
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.IsComplete).HasColumnName("isComplete");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("users"); // שם הטבלה במסד הנתונים
            
            entity.Property(e => e.Id).HasColumnName("id");
            
            entity.Property(e => e.Username)
                .HasColumnName("username")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(255); // אורך מוגדל עבור הצפנה בעתיד
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}


/*הקוד שסיפקת הוא מימוש של `DbContext` עבור Entity Framework Core, המייצג את מסד הנתונים שלך, במקרה זה `ToDoDbContext`. הנה הסבר על החלקים השונים בקוד:

1. **מבנה ה-DbContext**:
   - יש לך שני קונסטרוקטורים: אחד ברירת מחדל ואחד שמקבל `DbContextOptions<ToDoDbContext>`, שמאפשר הגדרת אפשרויות חיבור למסד הנתונים.

2. **DbSet<Item>**:
   - `public virtual DbSet<Item> Items { get; set; }` - מייצג את הטבלה `items` במסד הנתונים. `Item` הוא המודל שלך שמייצג את הפריטים שברצונך לנהל.

3. **OnConfiguring**:
   - בשיטה זו אתה מגדיר את חיבור המסד נתונים. אתה משתמש במחרוזת חיבור בשם `ToDoDB`, אך שים לב שהשיטה הזו לא צריכה להיות בשימוש אם אתה מגדיר את החיבור ב-`appsettings.json` וב-`Startup.cs`.

4. **OnModelCreating**:
   - כאן אתה מגדיר את המודל של הטבלה. אתה קובע את הקולציה והקידוד של הטבלה, וגם מגדיר את המפתח הראשי (`HasKey`) ואת המאפיינים של הישות `Item`.

5. **OnModelCreatingPartial**:
   - מתודה חלקית שמאפשרת להוסיף קוד נוסף למודל אם יש צורך, מבלי לשנות את הקוד הקיים.

אם אתה מתכוון להשתמש במחרוזת חיבור מ-`appsettings.json`, כדאי להסיר את השימוש ב-`OnConfiguring` כדי למנוע בלבול. אם יש לך שאלות נוספות או אם אתה נתקל בבעיות, אני כאן לעזור.
*/
