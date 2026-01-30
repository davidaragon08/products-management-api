using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsManagement.Application.DTOs
{
    public class ProductQueryDto
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        // Filtro simple por nombre (contains)
        public string? Search { get; set; }

        // Ordenación
        // Campos permitidos: name, price, quantity
        public string? SortBy { get; set; }

        // asc | desc
        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
        public string? SortDirection { get; set; }
    }
}
