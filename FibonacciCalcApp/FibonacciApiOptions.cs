using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciCalcApp
{   

    internal class FibonacciApiOptions
    {
        [Required]
        public const string Name = nameof(FibonacciApiOptions);
        [Required]
        public string Url { get; set; } = "http://localhost/";
    }
}
