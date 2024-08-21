using Sim.Application.UseCases.CreateLogicModel;
using Sim.Domain.UiSchematic;

namespace Sim.Application;

public class SimModelManager(ICreateLogicModel creatorUseCase) : ISimModel
{
    readonly ICreateLogicModel creator = creatorUseCase;
    public async Task Create(UiSchemeModel elements) 
    {
        var model = creator.Generate(elements);

        await model.Evaluate();

        var updated = await model.Evaluate();
    }

}

