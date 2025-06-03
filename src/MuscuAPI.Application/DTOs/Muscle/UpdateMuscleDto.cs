using System.ComponentModel.DataAnnotations;

namespace MuscuAPI.Application.DTOs.Muscle;

public class UpdateMuscleDto : CreateMuscleDto
{
    public bool IsActive { get; set; } = true;
}