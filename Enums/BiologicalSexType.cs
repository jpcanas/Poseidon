using System.ComponentModel.DataAnnotations;

namespace Poseidon.Enums
{
    public enum BiologicalSexType
    {
        [Display(Name = "Male")]
        Male = 1,

        [Display(Name = "Female")]
        Female,

        [Display(Name = "Prefer not to say")]
        PreferNotToSay   
    }
}
