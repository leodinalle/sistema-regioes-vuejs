using System.ComponentModel.DataAnnotations;


namespace RegionAPI.Model
{
    public class Region
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Uf { get; set; }

        [Required]
        public string Nome { get; set; }

        public bool Ativo { get; set; }
    }
}
