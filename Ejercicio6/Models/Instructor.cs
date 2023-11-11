using System.ComponentModel.DataAnnotations;

namespace Ejercicio6;

public class Instructor
{
    public uint id_persona { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Primer Nombre")]
    [MinLength(2, ErrorMessage = "El campo {0} no puede ser menor a 2 caracteres")]
    public string? nombre_uno { get; set; }
    
    [Display(Name = "Segundo Nombre")]
    public string? nombre_dos { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Apellido Paterno")]
    [MinLength(2, ErrorMessage = "El campo {0} no puede ser menor a 2 caracteres")]
    public string? apellido_uno { get; set; }
    
    [Display(Name = "Apellido Materno")]
    public string? apellido_dos { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Fecha de Nacimiento")]
    public DateTime? D_nacimiento { get; set; }
    
    [Display(Name = "Tipo de Rol")]
    public string? tipo_rol { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Folio")]
    [MinLength(2, ErrorMessage = "El campo {0} no puede ser menor a 2 caracteres")]
    public string? folio { get; set; }
}