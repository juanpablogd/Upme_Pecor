//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NSPecor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public partial class MUB_CLASE_CP
    {
        public MUB_CLASE_CP()
        {
            this.MUB_PROYECTOS_PECOR = new HashSet<MUB_PROYECTOS_PECOR>();
        }
        [Key]
        public long ID_CLASE_CP { get; set; }
        [DisplayName("DESCRIPCIÓN")]
        [Required]
        public string NOM_CLASE_CP { get; set; }
        public Nullable<long> ID_USUARIO_ACTUALIZACION { get; set; }
        [DisplayName("FECHA ACTUALIZACIÓN")]
        public Nullable<System.DateTime> FECHA_ACTUALIZACION { get; set; }
    
        public virtual ICollection<MUB_PROYECTOS_PECOR> MUB_PROYECTOS_PECOR { get; set; }
    }
}
