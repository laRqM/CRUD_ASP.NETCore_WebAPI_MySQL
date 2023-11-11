using System.ComponentModel.DataAnnotations;

namespace Ejercicio6;

public class Alumno
{
    // Esta clase es el modelo para mostrar la unión de las tablas persona y alumno.
    // De igual forma se utiliza para actualizar y crear datos en esas tablas.
    
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
    [Display(Name = "Matrícula")]
    [MinLength(2, ErrorMessage = "El campo {0} no puede ser menor a 2 caracteres")]
    public string? Matricula { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Carrera")]
    [MinLength(2, ErrorMessage = "El campo {0} no puede ser menor a 2 caracteres")]
    public string? Carrera {get; set;}
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [Display(Name = "Semestre")]
    public string? Semestre {get; set;}
    
    [Display(Name = "Especialidad")]
    public string? Especialidad {get; set;}
}