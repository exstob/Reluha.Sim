using Sim.Application.UseCases.CreateSchemeLogicModel;
using Sim.Domain.UiSchematic;

namespace Sim.Application;

public class SimModelManager: ISimModel
{
    static public async Task Create(UiSchemeModel elements) 
    {
        var model = CreateSchemeLogicModel.Generate(elements);

        await model.Evaluate();

        var updated = await model.Evaluate();
    }

}

