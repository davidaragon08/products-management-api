using System;

namespace ProductsManagement.Domain.Entities
{
    public class Product
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Quantity { get; set; }

        // Concurrency token
        public int Version { get; set; } = 1;

    }
}