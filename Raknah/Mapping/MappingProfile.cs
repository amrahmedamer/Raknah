using Raknah.Contracts.Authentication;

namespace Raknah.Mapping;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email.Split('@', StringSplitOptions.None)[0]);


    }
}
