using System;
namespace CatalogService.Models{


public class ItemsNotFoundException : Exception
{
    public ItemsNotFoundException()
    {
    }

    public ItemsNotFoundException(string message)
        : base(message)
    {
    }

    public ItemsNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
}