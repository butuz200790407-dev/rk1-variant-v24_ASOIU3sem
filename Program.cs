using System;
using System.Collections.Generic;
using System.Linq;

namespace RK1_App;

/// <summary>
/// Глава книги (Класс 1, сторона связи "многие")
/// </summary>
public class Chapter
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PagesCount { get; set; }
    public int BookId { get; set; }

    public Chapter(int id, string title, int pagesCount, int bookId)
    {
        Id = id;
        Title = title;
        PagesCount = pagesCount;
        BookId = bookId;
    }
}

/// <summary>
/// Книга (Класс 2, сторона связи "один")
/// </summary>
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public Book(int id, string title)
    {
        Id = id;
        Title = title;
    }
}

/// <summary>
/// Реализация связи многие-ко-многим
/// </summary>
public class BookChapter
{
    public int BookId { get; set; }
    public int ChapterId { get; set; }

    public BookChapter(int bookId, int chapterId)
    {
        BookId = bookId;
        ChapterId = chapterId;
    }
}

internal class Program
{
    private static void Main()
    {
        // Список книг
        var books = new List<Book>
        {
            new(1, "Архитектура ЭВМ"),
            new(2, "Алгоритмы и структуры данных"),
            new(3, "Паттерны проектирования"),
            new(11, "Архитектура распределенных систем"),
            new(22, "Алгоритмы оптимизации"),
            new(33, "Паттерны микросервисов")
        };

        // Список глав
        var chapters = new List<Chapter>
        {
            new(1, "Анализ производительности процессоров", 45, 1),
            new(2, "Асимптотический анализ сложности", 28, 2),
            new(3, "Фабричный метод и абстрактная фабрика", 18, 3),
            new(4, "Адаптер и фасад", 24, 3),
            new(5, "Одиночка и прототип", 12, 3)
        };

        // Связи многие-ко-многим
        var booksChapters = new List<BookChapter>
        {
            new(1, 1),
            new(2, 2),
            new(3, 3),
            new(3, 4),
            new(3, 5),

            new(11, 1),
            new(22, 2),
            new(33, 3),
            new(33, 4),
            new(33, 5)
        };

        // Соединение данных один-ко-многим (LINQ)
        var oneToMany = from b in books
            join c in chapters on b.Id equals c.BookId
            select new
            {
                ChapterTitle = c.Title,
                c.PagesCount,
                BookTitle = b.Title,
                BookId = b.Id
            };

        // Соединение данных многие-ко-многим (LINQ)
        var manyToMany = from bc in booksChapters
            join b in books on bc.BookId equals b.Id
            join c in chapters on bc.ChapterId equals c.Id
            select new
            {
                ChapterTitle = c.Title,
                c.PagesCount,
                BookTitle = b.Title
            };

        // --- Задание В1 ---
        // Главы, название которых начинается с буквы "А", и названия их книг
        Console.WriteLine("=== Задание В1 ===");
        var res1 = oneToMany
            .Where(x => x.ChapterTitle.StartsWith("А", StringComparison.OrdinalIgnoreCase))
            .Select(x => new { x.ChapterTitle, x.BookTitle });

        foreach (var item in res1)
        {
            Console.WriteLine($"Глава: \"{item.ChapterTitle}\" | Книга: \"{item.BookTitle}\"");
        }

        // --- Задание В2 ---
        // Книги с минимальным количеством страниц в главе, отсортированные по возрастанию
        Console.WriteLine("\n=== Задание В2 ===");
        var res2 = books
            .Select(b =>
            {
                var bookChapters = oneToMany.Where(x => x.BookId == b.Id).ToList();
                return new
                {
                    BookTitle = b.Title,
                    MinPages = bookChapters.Any() ? bookChapters.Min(x => x.PagesCount) : (int?)null
                };
            })
            .Where(x => x.MinPages.HasValue)
            .OrderBy(x => x.MinPages!.Value);

        foreach (var item in res2)
        {
            Console.WriteLine($"Книга: \"{item.BookTitle}\" | Мин. страниц в главе: {item.MinPages}");
        }

        // --- Задание В3 ---
        // Все связанные главы и книги (многие-ко-многим), отсортированные по названию главы
        Console.WriteLine("\n=== Задание В3 ===");
        var res3 = manyToMany
            .OrderBy(x => x.ChapterTitle)
            .ThenBy(x => x.BookTitle);

        foreach (var item in res3)
        {
            Console.WriteLine($"Глава: \"{item.ChapterTitle}\" ({item.PagesCount} стр.) ---> Книга: \"{item.BookTitle}\"");
        }
    }
}