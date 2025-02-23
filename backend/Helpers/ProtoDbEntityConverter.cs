using Bookx.Models;
using Bookx.ProtoServices;
using Google.Protobuf.WellKnownTypes;

namespace Bookx.Helpers;

public static class ProtoDbEntityConverter
{
    public static SingleAuthor DbToProtoAuthor(Author dbAuthor)
    {
        return new SingleAuthor()
        {
            Id = dbAuthor.Id,
            FirstName = dbAuthor.FirstName,
            LastName = dbAuthor.LastName,
            Birthdate = Timestamp.FromDateTime(dbAuthor.Birthdate.Value)
        };
    }

    public static SingleLanguage DbToProtoLanguage(Language dbLanguage)
    {
        return new SingleLanguage()
        {
            Id = dbLanguage.Id,
            Name = dbLanguage.Name
        };
    }

    public static SinglePublisher DbToProtoPublisher(Publisher dbPublisher)
    {
        return new SinglePublisher()
        {
            Id = dbPublisher.Id,
            Name = dbPublisher.Name
        };
    }

    public static SingleGenre DbToProtoGenre(Genre dbGenre)
    {
        return new SingleGenre()
        {
            Id = dbGenre.Id,
            Name = dbGenre.Name
        };
    }

    public static SingleBook DbToProtoBook(Book dbBook)
    {
        var protoLanguage = DbToProtoLanguage(dbBook.Language);
        var protoPublisher = DbToProtoPublisher(dbBook.Publisher);


        var protoBook = new SingleBook()
        {
            Isbn = dbBook.Isbn,
            Title = dbBook.Title,
            //CoverPath
            NumberOfPages = dbBook.NumerOfPages,
            ShopLink = dbBook.ShopLink,
            Blurb = dbBook.Blurb,
            Language = protoLanguage,
            Publisher = protoPublisher,
        };

        foreach (var dbGenre in dbBook.Genres)
        {
            protoBook.Genres.Add(DbToProtoGenre(dbGenre));
        }

        foreach (var dbAuthor in dbBook.Authors)
        {
            protoBook.Authors.Add(DbToProtoAuthor(dbAuthor));
        }

        return protoBook;
    }
}
