namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string input = Console.ReadLine();
            //string result = GetBooksReleasedBefore(db,input);
            //Console.WriteLine(result);
            int result=RemoveBooks(db);
            Console.WriteLine(result);

        }

        //ZAD--2
        public static string GetBooksByAgeRestriction(
                  BookShopContext context, string command)
        {
            List<string> bookTitles = context
                .Books
                .AsEnumerable()
                .Where(b => b.AgeRestriction.ToString()
                      .ToLower() == command.ToLower())
                .Select(b => b.Title)
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //ZAD--3
        public static string GetGoldenBooks(
                        BookShopContext context)
        {
            List<string> bookTitle = context
                .Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();


            return String.Join(Environment.NewLine, bookTitle);
        }

        //ZAD--4
        public static string GetBooksByPrice(
                               BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40M)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:F2}");
            }
            return sb.ToString().TrimEnd();
        }



        //ZAD--5
        public static string GetBooksNotReleasedIn(
                       BookShopContext context, int year)
        {
            List<string> booksNotReleaseIn = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)  //Value-zachtoto moje da e NULL
                .Select(b => b.Title)
                .ToList();

            return String.Join(Environment.NewLine, booksNotReleaseIn);
        }

        //ZAD--6
        public static string GetBooksByCategory(
                   BookShopContext context, string input)
        {
            string[] categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            List<string> bookTitles = new List<string>();
            foreach (string cat in categories)
            {
                List<string> curBookTitles = context
                    .Books
                    .Where(b => b.BookCategories.Any(bc =>
                      bc.Category.Name.ToLower() == cat))
                    .Select(b=> b.Title)
                    .ToList();

                bookTitles.AddRange(curBookTitles);
             }
            bookTitles = bookTitles
                .OrderBy(bt => bt)
                .ToList();

            return String.Join(Environment.NewLine, bookTitles);
        }

        //ZAD--7
        // public static string GetBooksReleasedBefore(
        //                   BookShopContext context, string date)
        // {
        //     StringBuilder sb = new StringBuilder();
        //
        //     var books=context
        //         .Books
        //         .Where(b => b.ReleaseDate)
        //         .Select(b=>new
        //         {
        //             b.Title,
        //             b.EditionType,
        //             b.Price
        //         })
        //         .OrderBy(books => books.ReleaseDate)
        //         .ToList();
        //
        //     foreach (var b in books)
        //     {
        //         sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
        //     }
        //
        //     return sb.ToString().TrimEnd();
        // }
        //

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < releaseDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new { b.Title, b.EditionType, b.Price })
                .ToList();

            return String.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));
        }

        //ZAD--8
        public static string GetAuthorNamesEndingIn(
                     BookShopContext context, string input)
        {
            List<string> authors = context
                .Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(a => a.FirstName + " " + a.LastName)
                .OrderBy(a => a)
                .ToList();

            return String.Join(Environment.NewLine, authors);
        }

        //ZAD--9

     public static string GetBookTitlesContaining(
                  BookShopContext context, string input)
     {
         List<string> titleContainString = new List<string>();
   
          titleContainString = context
               .Books
               .Where(b => b.Title.ToLower().Contains(input.ToLower()))
               .Select(b=>b.Title)
               .OrderBy(bt => bt)
               .ToList();
   
           return String.Join(Environment.NewLine,titleContainString);
     }

        //ZAD--10
        public static string GetBooksByAuthor(
                      BookShopContext context, string input)
        {

            StringBuilder sb = new StringBuilder();

            var booksByAuthor = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy( b=>b.BookId)
                .Select(b=>new
                {
                    b.Title,
                    b.Author.FirstName,
                    b.Author.LastName
                })
                .ToList();

            foreach (var b in booksByAuthor)
            {
                sb.AppendLine($"{b.Title} ({b.FirstName} {b.LastName})");
            }
            return sb.ToString().TrimEnd();
        }

        //ZAD--11
        public static int CountBooks(
            BookShopContext context, int lengthCheck)
        {
            int count = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return count;
        }


        //ZAD--12
        public static string CountCopiesByAuthor(
                               BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var authorCopies = context
                .Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    BookCopies = a.Books.Sum(b => b.Copies)
                })
            .OrderByDescending(a => a.BookCopies)
            .ToList();

            foreach (var a in authorCopies)
            {
                sb.AppendLine($"{a.FullName} - {a.BookCopies}");
            }
            return sb.ToString().TrimEnd();
        }

        //ZAD--13

        public static string GetTotalProfitByCategory(
                                   BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categoryProfits = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                                  .Select(cb => new
                                  {
                                      BookProfit = cb.Book.Copies * cb.Book.Price
                                  })
                                  .Sum(cb => cb.BookProfit)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var c in categoryProfits)
            {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:F2}");
            }
            return sb.ToString().TrimEnd();
        }


        //ZAD--14

     public static string GetMostRecentBooks(
                          BookShopContext context)
     {
         StringBuilder sb = new StringBuilder();
    
         var categoriesWMRBooks = context
             .Categories
             .Select(c => new
             {
                 CategoryName = c.Name,
                 MostRecentBooks = c.CategoryBooks
                                 .OrderByDescending(cb => cb.Book.ReleaseDate)
                                 .Take(3)
                                 .Select(cb => new
                                 {
                                     BookTitle = cb.Book.Title,
                                     ReleaseYear = cb.Book.ReleaseDate.Value.Year
                                 })
                                 .ToList()
             })
              .OrderBy(c => c.CategoryName)
              .ToList();
     
          foreach (var c in categoriesWMRBooks)
          {
              sb.AppendLine($"--{c.CategoryName}");
     
              foreach (var b in c.MostRecentBooks)
              {
                  sb.AppendLine($"{b.BookTitle} ({b.ReleaseYear})");
              }
          }
     
     
          return sb.ToString().TrimEnd();
      }
     
   

        //ZAD--15

        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUptate = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in booksToUptate)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }


        //ZAD--16
        // public static int RemoveBooks(
        //            BookShopContext context)
        // {
        //   // var booksRemoveds = context
        //   //     .Books
        //   //     .Where(b => b.Copies < 4200);
        //   //
        //   // foreach (var bR in booksRemoveds)
        //   // {
        //   //    context.BookCategories.Remove(bR.BookCategories.BookId);
        //   //     context.Books.Remove(bR);
        //   // }
        //   //
        //   //
        //   //
        //   // //
        //   // // context.BooksCategories.RemoveRange(booksRemoveds.BooksCategories.BookId);
        //   // // context.RemoveRange(booksRemoveds);
        //   // // context.SaveChanges();
        //   // //
        //   // context.SaveChanges();
        //   // return booksRemoveds.Count();
        //
        //
        // }
        //

        //  public static int RemoveBooks(BookShopContext context)
        //  {
        //      var books = context.Books.Where(b => b.Copies < 4200);
        //
        //      var result = books.Count();
        //
        //      context.Books.RemoveRange(books);
        //
        //      context.SaveChanges();
        //
        //      return result;
        //  }

        //15. Remove Books\
        public static int RemoveBooks(BookShopContext context)
        {
            var BookInMap = context.BooksCategories
                .Where(bc => bc.Book.Copies< 4200)
                .ToList();

            foreach (var book in BookInMap)
            {
                context.Remove(book);
            }

               var books = context.Books
              .Where(b => b.Copies < 4200).ToList();

                context.SaveChanges();

                int booksCount = books.Count();

            foreach (var book in books)
            {
                context.Remove(book);
            }

           context.SaveChanges();

            return booksCount;
        }


    }
}
