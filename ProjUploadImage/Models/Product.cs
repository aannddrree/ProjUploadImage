using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjUploadImage.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Nome da Imagem")]
        public string Image { get; set; }

        [NotMapped]
        [DisplayName("Imagem")]
        public IFormFile ImageFile { get; set; }
    }
}
