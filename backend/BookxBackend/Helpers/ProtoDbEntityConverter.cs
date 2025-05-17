using Bookx.Models;
using BookxProtos;
using Google.Protobuf.WellKnownTypes;

namespace Bookx.Helpers;

public static class ProtoDbEntityConverter
{
    public static SingleAuthor DbToProtoAuthor(Author dbAuthor)
    {
        var protoAuthor = new SingleAuthor()
        {
            Id = dbAuthor.Id,
            FirstName = dbAuthor.FirstName,
            LastName = dbAuthor.LastName,
        };

        if (dbAuthor.Birthdate != null)
            protoAuthor.Birthdate = Timestamp
                .FromDateTime(dbAuthor.Birthdate.Value.ToDateTime(TimeOnly.MinValue).ToUniversalTime());

        return protoAuthor;
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

        // TODO: coverpath
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

        protoBook.ReleaseDate = Timestamp
            .FromDateTime(dbBook.ReleaseDate.ToDateTime(TimeOnly.MinValue).ToUniversalTime());

        // TODO: fix genres
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

    public static ReadSingleOwnedBook DbToProtoOwnedBook(OwnedBook dbOwnedBook)
    {
        var protoBook = DbToProtoBook(dbOwnedBook.Book);

        var protoOwnedBook = new ReadSingleOwnedBook()
        {
            Book = protoBook,
            Rating = dbOwnedBook.Rating,
            Comment = dbOwnedBook.Comment,
            WouldRecommend = dbOwnedBook.WouldRecommend,
        };

        protoOwnedBook.AddedAt = Timestamp.FromDateTime(dbOwnedBook.AddedAt);

        foreach (Tag tag in dbOwnedBook.Tags)
        {
            protoOwnedBook.Tags.Add(DbToProtoTag(tag));
        }

        return protoOwnedBook;
    }

    public static ReadSingleTag DbToProtoTag(Tag dbTag)
    {
        return new ReadSingleTag()
        {
            Id = dbTag.Id,
            Name = dbTag.Name,
            Color = dbTag.Color
        };
    }
}
