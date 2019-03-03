//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Evento
    {
        public int id_evento { get; set; }
        public string codigo { get; set; }
        public Nullable<int> matricula_co { get; set; }
        public int id_estado { get; set; }
        [Required]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Solo se permite el uso de letras")]
        public string ponente { get; set; }
        [Required]
        public string titulo { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime fecha { get; set; }
        [Required]
        public string lugar { get; set; }
        [Required]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Solo se permite el uso de letras")]
        public string asesor { get; set; }
        [Required]
        public int id_tipo { get; set; }
        [Required]
        public string abstracto { get; set; }
    
        public virtual c_Estado_Evento c_Estado_Evento { get; set; }
        public virtual c_Tipo_Evento c_Tipo_Evento { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
